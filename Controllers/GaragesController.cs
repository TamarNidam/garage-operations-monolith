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

namespace Garage_Management.Controllers
{
    public class GaragesController : Controller
    {
        private readonly Garage_ManagementContext _context;

        public GaragesController(Garage_ManagementContext context)
        {
            _context = context;
        }

        // GET: Garages
        public async Task<IActionResult> Index(int userid)
        {
            try
            {
                var sql = "SELECT * FROM [Garage]";
                var garages =await _context.Garages.FromSqlRaw(sql).ToListAsync();
                var garageDTOs1 = await Task.WhenAll(garages
                        .Select(async g =>
                        {
                            GaragePermission permission = _context.GaragePermissions.FirstOrDefault(p =>
               p.UserId == userid && p.GarageId == g.GarageId);
                            if (permission == null)
                            {
                                return null;
                            }
                            return new GarageDTO
                            {
                                GarageId = g.GarageId,
                                GarageName = g.GarageName,
                                Address = g.Address,
                                PhoneNumber = g.PhoneNumber,
                                CanView = (bool)permission.CanView,
                                CanEdit = (bool)permission.CanEdit
                            };
                        }));
                var garageDTOs = garageDTOs1.Where(dto => dto != null).ToList();
                ViewBag.ActivateLayout = 0;
                return View(garageDTOs); 
                //return View(await _context.Garages.ToListAsync());
            }
            catch (Exception ex)
            {
                ViewBag.ActivateLayout = 2;
                return View("Error", ex); ;
            }
        
           
        }

        // GET: Garages/Details/5
        public async Task<IActionResult> Details(int userid, int? id)
        {
            try
            {

           
            if (id == null)
            {
                return NotFound();
            }

            var garage = await _context.Garages
                .FromSqlRaw("SELECT TOP 1 * FROM Garage WHERE GarageId = {0}", id)
                .FirstOrDefaultAsync();

                if (garage == null)
            {
                return NotFound();
            }

                GaragePermission permission = _context.GaragePermissions.FirstOrDefault(p =>
                               p.UserId == userid && p.GarageId == id);
                if (permission == null)
                {
                    return NotFound();
                }
                var garageDTO = new GarageDTO
                {
                    GarageId = garage.GarageId,
                    GarageName = garage.GarageName,
                    Address = garage.Address,
                    PhoneNumber = garage.PhoneNumber,
                    CanEdit = (bool)permission.CanEdit,
                    CanView = (bool)permission.CanView
                };
                ViewBag.ActivateLayout = 0;
                return View(garageDTO);
            }
            catch(Exception ex)
            {
                ViewBag.ActivateLayout = 2;
                return View("Error",ex);
            }
        }
        // GET: Garages/Create
        public IActionResult Create()
        {
            ViewBag.ActivateLayout = 0;
            return View();
        }

        // POST: Garages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int userid, [Bind("GarageId,GarageName,Address,PhoneNumber")] GarageDTO garageDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var g = await _context.Garages
                .FromSqlRaw("SELECT TOP 1 * FROM Garage WHERE GarageName = {0}", garageDTO.GarageName)
                .FirstOrDefaultAsync();
                    if (g != null)
                    {
                        ViewBag.ActivateLayout = 0;
                        ViewBag.ErrorMessage = "Garage name exist";
                        return View(garageDTO);
                    }
                    var maxGarageId = await _context.Garages.MaxAsync(c => (int?)c.GarageId) ?? 0;
                    var newGarageId = maxGarageId + 1;
                    var sql = $"INSERT INTO [Garage] (GarageId,GarageName,Address,PhoneNumber) VALUES ({newGarageId}, '{garageDTO.GarageName}', '{garageDTO.Address}', '{garageDTO.PhoneNumber}')";
                    await _context.Database.ExecuteSqlRawAsync(sql);

                    var maxPermissionId = await _context.GaragePermissions.MaxAsync(m => (int?)m.PermissionId) ?? 0;
                    var users = await _context.Users.ToListAsync();
                    foreach (var user in users)
                    {
                        maxPermissionId = maxPermissionId + 1;
                        var nPermission = new GaragePermission
                        {
                            PermissionId = maxPermissionId,
                            UserId = user.UserId,
                            GarageId = newGarageId,
                            CanView = false,
                            CanEdit = false
                        };
                        if (user.UserId == 0)
                        {
                            nPermission.CanView = true;
                            nPermission.CanEdit = true;
                        }
                        _context.GaragePermissions.Add(nPermission);

                    }
                    await _context.SaveChangesAsync();
                    return Redirect($"/Garages/Index?userid={userid}");
                }
                ViewBag.ActivateLayout = 0;
                return View(garageDTO);
            }
            catch (Exception ex)
            {
                ViewBag.ActivateLayout = 2;
                return View("Error", ex);
            }
        }

        // GET: Garages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            try { 
            if (id == null)
            {
                return NotFound();
            }

                var sql = $"SELECT GarageId,GarageName,Address,PhoneNumber FROM [Garage] WHERE GarageId = {id}";
                var garage = await _context.Garages.FromSqlRaw(sql).FirstOrDefaultAsync();

                if (garage == null)
            {
                return NotFound();
            }
                var garageDTO = new GarageDTO
                {
                    GarageId = garage.GarageId,
                    GarageName = garage.GarageName,
                    Address = garage.Address,
                    PhoneNumber = garage.PhoneNumber                    
                };
                ViewBag.ActivateLayout = 0;
                return View(garageDTO);
            }
            catch
            {
                ViewBag.ActivateLayout = 2;
                return View("Error");
            }
        }

        // POST: Garages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int userid, int id, [Bind("GarageId,GarageName,Address,PhoneNumber,CanView,CanEdit")] GarageDTO garageDTO)
        {
            try
            {

                // if (id != garage.GarageId)
                //{
                //    return NotFound();
                //}

                if (ModelState.IsValid)
                {

                    var sql = $"UPDATE [Garage] SET GarageName = '{garageDTO.GarageName}',Address = '{garageDTO.Address}',PhoneNumber = '{garageDTO.PhoneNumber}' WHERE GarageId = {garageDTO.GarageId}";
                    await _context.Database.ExecuteSqlRawAsync(sql);

                    return Redirect($"/Garages/Index?userid={userid}");
                }
                ViewBag.ActivateLayout = 0;
                return View(garageDTO);
            }
            catch (Exception ex)
            {
                ViewBag.ActivateLayout = 2;
                return View("Error", ex);
            }
        }

        // GET: Garages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var garage = await _context.Garages
                .FirstOrDefaultAsync(m => m.GarageId == id);
            if (garage == null)
            {
                return NotFound();
            }

            var garageDTO = new GarageDTO
            {
                GarageId = garage.GarageId,
                GarageName = garage.GarageName,
                Address = garage.Address,
                PhoneNumber = garage.PhoneNumber
            };

            var visits = _context.GarageVisits.Where(v => v.GarageId == id).ToList();
            var visitDTOs = visits
                .Join(_context.Customers, v => v.CustomerId, c => c.CustomerId, (v, c) => new { Visit = v, Customer = c })
                .Select(vc => new GarageVisitDTO
                {
                    VisitId = vc.Visit.VisitId,
                    CustomerId = vc.Visit.CustomerId,
                    CustomerName = vc.Customer.FirstName,
                    GarageId = vc.Visit.GarageId,
                    GarageName = garage.GarageName,
                    VisitDate = vc.Visit.VisitDate,
                    ServiceDescription = vc.Visit.ServiceDescription,
                    TotalCost = vc.Visit.TotalCost

                }).ToList();

            var viewModel = new Garage_Visits_DTO
            {
                Garage = garageDTO,
                Visits = visitDTOs
            };

            ViewBag.ActivateLayout = 0;
            return View(viewModel);
        }

        // POST: Garages/Delete/5
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
                if (!GarageExists(id))
                {
                    ViewBag.ActivateLayout = 2;
                    return View("Error");
                }
                var sql = $"DELETE [GaragePermissions] WHERE GarageId = {id}";
                await _context.Database.ExecuteSqlRawAsync(sql);
                sql = $"DELETE [GarageVisits] WHERE GarageId = {id}";
                await _context.Database.ExecuteSqlRawAsync(sql);
                sql = $"DELETE [Garage] WHERE GarageId = {id}";
                await _context.Database.ExecuteSqlRawAsync(sql);

                if (GarageExists(id))
                {
                    ViewBag.ActivateLayout = 2;
                    return View("Error");
                }


                return Redirect($"/Garages/Index?userid={0}");
            }
            catch (Exception ex)
            {
                ViewBag.ActivateLayout = 2;
                return View("Error", ex);
            }
        }


        private bool GarageExists(int id)
        {
            return _context.Garages.Any(e => e.GarageId == id);
        }
    }
}
