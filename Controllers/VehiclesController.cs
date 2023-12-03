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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Garage_Management.Controllers
{
    public class VehiclesController : Controller
    {
        private const string sql_customer = "SELECT * FROM Customers WHERE CustomerId = {0}";
        private readonly Garage_ManagementContext _context;

        public VehiclesController(Garage_ManagementContext context)
        {
            _context = context;
        }

        // GET: Vehicles
        public async Task<IActionResult> Index(int userid, int? id)
        {
            try
            {
                string sql;
                if(id == null)
                {
 sql = "SELECT * FROM Vehicles";
                
                }
                else
                {
                    sql = $"SELECT * FROM Vehicles WHERE OwnerId = {id}";
                }
                var Vehicles = await _context.Vehicles.FromSqlRaw(sql).ToListAsync();
                var vehicleDTOs = Vehicles
                    .Select(async v => new VehicleDTO
                    {
                        VehicleId = v.VehicleId,
                        Make = v.Make,
                        Model = v.Model,
                        Year = v.Year,
                        Vin = v.Vin,
                        Mileage = v.Mileage,
                        LastServiceDate = v.LastServiceDate,
                        OwnerId = v.OwnerId,
                        OwnerName = await _context.Customers.FromSqlRaw(sql_customer, v.OwnerId).Select(c => c.FirstName).FirstOrDefaultAsync()
                                     }).Select(t => t.Result).ToList();
                if(id == null)
                {
                    ViewBag.ifCanEdit = false;
                    ViewBag.ifCanView = false;
                }
             
                else
                {
var caneditSql = $"SELECT * FROM [Permissions] WHERE UserId = {userid} AND CustomerId = {id}";
                var canedit = await _context.Permissions.FromSqlInterpolated($"SELECT * FROM [Permissions] WHERE UserId = {userid} AND CustomerId = {id}").FirstOrDefaultAsync();
                 
                if (canedit.CanEdit == true)
                {
                    ViewBag.ifCanEdit = true; 
                }
                else
                {
                    ViewBag.ifCanEdit = false;
                }
                if(canedit.CanView == true)
                    {
                        ViewBag.ifCanView = true;
                    }
                    else
                    {
                        ViewBag.ifCanView = false;
                    }
                }
                
                ViewBag.ActivateLayout = 0;
                return View(vehicleDTOs);
            }
            catch (Exception ex)
            {
                ViewBag.ActivateLayout = 2;
                return View("Error",ex);
            }
        }

        // GET: Vehicles/Details/5
        public async Task<IActionResult> Details(int userid, int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var vehicle = await _context.Vehicles
                .FromSqlRaw("SELECT TOP 1 * FROM Vehicles WHERE VehicleId = {0}", id)
                .FirstOrDefaultAsync();
                if (vehicle == null)
                {
                    return NotFound();
                }

                var vehicleDTO = new VehicleDTO
                {
                    VehicleId = vehicle.VehicleId,
                    Make = vehicle.Make,
                    Model = vehicle.Model,
                    Year = vehicle.Year,
                    Vin = vehicle.Vin,
                    Mileage = vehicle.Mileage,
                    LastServiceDate = vehicle.LastServiceDate,
                    OwnerId = vehicle.OwnerId,
                    OwnerName = await _context.Customers.FromSqlRaw(sql_customer, vehicle.OwnerId).Select(c => c.FirstName).FirstOrDefaultAsync()
                                   };
                ViewBag.ActivateLayout = 0;
                return View(vehicleDTO);
            }
            catch(Exception ex)
            {
                ViewBag.ActivateLayout = 2;
                return View("Error",ex);
            }
        }
            

        // GET: Vehicles/Create
        public IActionResult Create(int userid, int? id)
        {
            if(id == null)
            {
ViewData["OwnerId"] = new SelectList(_context.Customers, "CustomerId", "FirstName");
            }
            else
            {
                var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == id);

                if (customer != null)
                {
                    ViewData["OwnerId"] = customer.FirstName;
                }
                
            }
            ViewBag.ActivateLayout = 0;
            return View();
        }

        // POST: Vehicles/Create
         [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int userid,int? id ,[Bind("VehicleId,Make,Model,Year,Vin,Mileage,LastServiceDate,OwnerId,OwnerName")] VehicleDTO vehicleDTO)
        {
                       try
            {
                if (ModelState.IsValid)
                {
                    var u = await _context.Vehicles
                .FromSqlRaw("SELECT TOP 1 * FROM Vehicles WHERE Vin = {0}", vehicleDTO.Vin)
                .FirstOrDefaultAsync();
                    if (u != null)
                    {
                        ViewBag.ActivateLayout = 0;
                        ViewBag.ErrorMessage = "VIN exist";
                        return View(vehicleDTO);
                    }

                    var maxVehicleId = await _context.Vehicles.MaxAsync(v => (int?)v.VehicleId) ?? 0;
                    var newVehicleId = maxVehicleId + 1;
                    var sql = $"INSERT INTO [Vehicles] (VehicleId, Make, Model, Year, Vin, Mileage, LastServiceDate, OwnerId) VALUES ({newVehicleId}, '{vehicleDTO.Make}', '{vehicleDTO.Model}', {vehicleDTO.Year}, '{vehicleDTO.Vin}', {vehicleDTO.Mileage}, '{vehicleDTO.LastServiceDate}' ,{vehicleDTO.OwnerId})";
                    await _context.Database.ExecuteSqlRawAsync(sql);
                    
                    return Redirect($"/Vehicles/Index?userid={userid}");
                }

ViewData["OwnerId"] = new SelectList(_context.Customers, "CustomerId", "FirstName", vehicleDTO.OwnerId);

                
                ViewBag.ActivateLayout = 0;
                return View(vehicleDTO);
            }
            catch (Exception ex)
            {
                return View("error", ex);
            }
        }

        // GET: Vehicles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var sql = $"SELECT * FROM [Vehicles] WHERE VehicleId = {id}";
                var vehicle = await _context.Vehicles.FromSqlRaw(sql).FirstOrDefaultAsync();
                if (vehicle == null)
                {
                    return NotFound();
                }
                var vehicleDTO = new VehicleDTO
                {
                    VehicleId = vehicle.VehicleId,
                    Make = vehicle.Make,
                    Model = vehicle.Model,
                    Year = vehicle.Year,
                    Vin = vehicle.Vin,
                    Mileage = vehicle.Mileage,
                    LastServiceDate = vehicle.LastServiceDate,
                    OwnerId = vehicle.OwnerId,
                    OwnerName = await _context.Customers.FromSqlRaw(sql_customer, vehicle.OwnerId).Select(c => c.FirstName).FirstOrDefaultAsync()
                };
                ViewData["OwnerId"] = new SelectList(_context.Customers, "CustomerId", "FirstName", vehicle.OwnerId);
                ViewBag.ActivateLayout = 0;
                return View(vehicleDTO);
            }
            catch (Exception ex)
            {
                ViewBag.ActivateLayout = 2;
                return View("Error", ex);
            }
        }

        // POST: Vehicles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int userid, int id, [Bind("VehicleId,Make,Model,Year,Vin,Mileage,LastServiceDate,OwnerId,OwnerName")] VehicleDTO vehicleDTO)
        {
            try
            {
                //if (id != vehicleDTO.VehicleId)
                //{
                //    return NotFound();
                //}

                if (ModelState.IsValid)
                {
                    var sql = $"UPDATE [Vehicles] SET Make = '{vehicleDTO.Make}', Model = '{vehicleDTO.Model}',Year = {vehicleDTO.Year}, Vin = '{vehicleDTO.Vin}', Mileage = {vehicleDTO.Mileage}, LastServiceDate = '{vehicleDTO.LastServiceDate}', OwnerId = {vehicleDTO.OwnerId} WHERE VehicleId = {vehicleDTO.VehicleId}";
                    await _context.Database.ExecuteSqlRawAsync(sql);

                    return Redirect($"/Home/Index?userid={userid}");
                }
                ViewData["OwnerId"] = new SelectList(_context.Customers, "CustomerId", "FirstName", vehicleDTO.OwnerId);
                ViewBag.ActivateLayout = 0;
                return View(vehicleDTO);
            }
            catch
            {
                ViewBag.ActivateLayout = 2;
                return View("Error");
            }
        }

        // GET: Vehicles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {

                if (id == null)
                {
                    return NotFound();
                }

                var vehicle = await _context.Vehicles.FindAsync(id);

                if (vehicle == null)
                {
                    return NotFound();
                }

                var vehicleDTO = new VehicleDTO
                {
                    VehicleId = vehicle.VehicleId,
                    Make = vehicle.Make,
                    Model = vehicle.Model,
                    Year = vehicle.Year,
                    Vin = vehicle.Vin,
                    Mileage = vehicle.Mileage,
                    LastServiceDate = vehicle.LastServiceDate,
                    OwnerId = vehicle.OwnerId,
                    OwnerName = await _context.Customers.FromSqlRaw(sql_customer, vehicle.OwnerId).Select(c => c.FirstName).FirstOrDefaultAsync()
                                    };
                ViewBag.ActivateLayout = 0;
                return View(vehicleDTO);
            }
            catch(Exception ex) 
            {
                ViewBag.ActivateLayout = 2;
                return View("Error",ex);
            }
        }

        // POST: Vehicles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int userid, int id)
        {
            try
            {
                //if (id == null)
                //{
                //    return NotFound();
                //}

               
                if (!VehicleExists(id))
                {
                    return NotFound();
                }

                await _context.Database.ExecuteSqlRawAsync($"DELETE FROM Vehicles WHERE VehicleId = {id}");

                if (VehicleExists(id))
                {
                    return NotFound();
                }

                return Redirect($"/Home/Index?userid={userid}");
            }
            catch(Exception ex)
            {
                ViewBag.ActivateLayout = 2;
                return View("Error",ex);
            }
        }

        private bool VehicleExists(int id)
        {
            return _context.Vehicles.Any(e => e.VehicleId == id);
        }
    }
}
