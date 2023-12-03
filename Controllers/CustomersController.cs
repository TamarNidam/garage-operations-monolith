using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Garage_Management.Models;
using Garage_Management.DTO;
using System.Net;
using Humanizer;


namespace Garage_Management.Controllers
{
    public class CustomersController : Controller
    {
        private readonly Garage_ManagementContext _context;

        public CustomersController(Garage_ManagementContext context)
        {
            _context = context;
        }

        // GET: Customers
        public async Task<IActionResult> Index(int userid)
        {
            try
            {
                var sql = "SELECT * FROM [Customers]";
                var customers = await _context.Customers.FromSqlRaw(sql).ToListAsync();

                var customerDTOsTasks = customers
                        .Select( c =>
                        {
                            Permission permission = _context.Permissions.FirstOrDefault(p =>
                p.UserId == userid && p.CustomerId == c.CustomerId);
                            if (permission == null)
                            {
                                return null;
                            }
                            return new CustomerDTO
                            {
                                CustomerId = c.CustomerId,
                                FirstName = c.FirstName,
                                LastName = c.LastName,
                                Email = c.Email,
                                Phone = c.Phone,
                                Address = c.Address,
                                CanView = (bool)permission.CanView,
                                CanEdit = (bool)permission.CanEdit
                                           };
                                                   
                        }).ToList();
                var customerDTOs = customerDTOsTasks.Where(dto => dto != null).ToList();
                ViewBag.ActivateLayout = 0;
                return View(customerDTOs);
            }
            catch
            {
                ViewBag.ActivateLayout = 2;
                return View("Error");
            }
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int userid, int? id)
        {         
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var customer = await _context.Customers
                .FromSqlRaw("SELECT TOP 1 * FROM Customers WHERE CustomerId = {0}", id)
                .FirstOrDefaultAsync();

                if (customer == null)
                {
                    return NotFound();
                }
                Permission permission = _context.Permissions.FirstOrDefault(p =>
               p.UserId == userid && p.CustomerId == id);
                if (permission == null)
                {
                    return NotFound();
                }
                var customerDTO = new CustomerDTO
                {
                    CustomerId = customer.CustomerId,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Email = customer.Email,
                    Phone = customer.Phone,
                    Address = customer.Address,
                    CanEdit = (bool)permission.CanEdit
                };
                ViewBag.ActivateLayout = 0;
                return View(customerDTO);
            }
            catch
            {
                ViewBag.ActivateLayout = 2;
                return View("Error");
            }
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            ViewBag.ActivateLayout = 0;
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int userid, [Bind("CustomerId,FirstName,LastName,Email,Phone,Address")] CustomerDTO customerDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var c = await _context.Customers
                .FromSqlRaw("SELECT TOP 1 * FROM Customers WHERE Email = {0}", customerDTO.Email)
                .FirstOrDefaultAsync();
                    if (c != null)
                    {
                        ViewBag.ActivateLayout = 0;
                        ViewBag.ErrorMessage = "Customer Email exist";
                        return View(customerDTO);
                    }
                    var maxCustomerId = await _context.Customers.MaxAsync(c => (int?)c.CustomerId) ?? 0;
                    var newCustomerId = maxCustomerId + 1;
                    var sql = $"INSERT INTO [Customers] (CustomerId,FirstName,LastName,Email,Phone,Address) VALUES ({newCustomerId}, '{customerDTO.FirstName}', '{customerDTO.LastName}', '{customerDTO.Email}', '{customerDTO.Phone}', '{customerDTO.Address}')";
                    await _context.Database.ExecuteSqlRawAsync(sql);

                    var maxPermissionId = await _context.Permissions.MaxAsync(m => (int?)m.PermissionId) ?? 0;
                    var users = await _context.Users.ToListAsync();
                    foreach (var user in users)
                    {
                        maxPermissionId = maxPermissionId + 1;
                        var nPermission = new Permission
                        {
                            PermissionId = maxPermissionId,
                            UserId = user.UserId,
                            CustomerId = newCustomerId,
                            CanView = false,
                            CanEdit = false
                        };
                        if (user.UserId == 0)
                        {
                            nPermission.CanView = true;
                            nPermission.CanEdit = true;
                        }
                        _context.Permissions.Add(nPermission);

                    }
                    await _context.SaveChangesAsync();
                    return Redirect($"/Customers/Index?userid={userid}");
                }
                ViewBag.ActivateLayout = 0;
                return View(customerDTO);
            }
            catch(Exception ex)
            {
                ViewBag.ActivateLayout = 2;
                return View("Error",ex);
            }
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id == null)
            {
                return NotFound();
            }

                var sql = $"SELECT CustomerId,FirstName,LastName,Email,Phone,Address FROM [Customers] WHERE CustomerId = {id}";
                var customer = await _context.Customers.FromSqlRaw(sql).FirstOrDefaultAsync();

                if (customer == null)
            {
                return NotFound();
            }

                var customerDTO = new CustomerDTO
                {
                    CustomerId = customer.CustomerId,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Email = customer.Email,
                    Phone = customer.Phone,
                    Address = customer.Address
                };
                ViewBag.ActivateLayout = 0;
                return View(customerDTO);
            }
            catch
            {
                ViewBag.ActivateLayout = 2;
                return View("Error");
            }
        }

        // POST: Customers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int userid,int id, [Bind("CustomerId,FirstName,LastName,Email,Phone,Address,CanView,CanEdit")] CustomerDTO customerDTO)
        {
            try
            {
                //if (id != customerDTO.CustomerId)
                //{
                //    return NotFound();
                //}

                if (ModelState.IsValid)
                {
                    var sql = $"UPDATE [Customers] SET FirstName = '{customerDTO.FirstName}',LastName = '{customerDTO.LastName}',Email = '{customerDTO.Email}',Phone = '{customerDTO.Phone}',Address = '{customerDTO.Address}' WHERE CustomerId = {customerDTO.CustomerId}";
                    await _context.Database.ExecuteSqlRawAsync(sql);

                    return Redirect($"/Customers/Index?userid={userid}");
                }
                ViewBag.ActivateLayout = 0;
                return View(customerDTO);
            }
            catch (Exception ex)
            {
                ViewBag.ActivateLayout = 2;
                return View("Error",ex);
            }
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            var customerDTO = new CustomerDTO
            {
                CustomerId = customer.CustomerId,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                Phone = customer.Phone,
                Address = customer.Address
            };

            var vehicles = _context.Vehicles.Where(v => v.OwnerId == id).ToList();
            var vehicleDTOs = vehicles
                    .Select(v => new VehicleDTO
                    {
                           VehicleId= v.VehicleId,
                           Make = v.Make,
                           Model = v.Model,
                           Year = v.Year,
                           Vin = v.Vin,
                           Mileage = v.Mileage,
                           LastServiceDate = v.LastServiceDate,
                        OwnerId = v.OwnerId,
                        OwnerName = customer.FirstName
                    }).ToList();

            var viewModel = new CustomerVehiclesDTO
            {
                Customer = customerDTO,
                Vehicles = vehicleDTOs
            };

            return View(viewModel);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int userid, int id)
        {
            try
            {
                if (id == 0)
                {
                    return NotFound();
                }
                if (!CustomerExists(id))
                {
                    ViewBag.ActivateLayout = 2;
                    return View("Error"); 
                }
                var sql = $"DELETE [Permissions] WHERE CustomerId = {id}";
                await _context.Database.ExecuteSqlRawAsync(sql);
                sql = $"DELETE [Vehicles] WHERE OwnerId = { id }";
                await _context.Database.ExecuteSqlRawAsync(sql);
                 sql = $"DELETE [Customers] WHERE CustomerId = {id}";
                await _context.Database.ExecuteSqlRawAsync(sql);

                if (CustomerExists(id))
                {
                    ViewBag.ActivateLayout = 2;
                    return View("Error");
                }
                

                return Redirect($"/Customers/Index?userid={0}");
            }
            catch (Exception ex)
            {
                ViewBag.ActivateLayout = 2;
                return View("Error",ex);
            }
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }
    }
}
