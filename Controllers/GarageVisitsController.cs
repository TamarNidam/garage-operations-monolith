using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Garage_Management.Models;
using Garage_Management.DTO;

namespace Garage_Management.Controllers
{
    public class GarageVisitsController : Controller
    {
        private const string sql_customer = "SELECT * FROM Customers WHERE CustomerId = {0}";
        private const string sql_garage = "SELECT * FROM Garage WHERE GarageId = {0}";
        private const string sql_permission = "SELECT * FROM GaragePermissions WHERE UserId = {0} AND GarageId = {1}";
        private readonly Garage_ManagementContext _context;

        public GarageVisitsController(Garage_ManagementContext context)
        {
            _context = context;
        }

        // GET: GarageVisits
        public async Task<IActionResult> Index(int userid, int? id)
        {
            try
            {
                string sql;
                if (id == null)
                {
                    sql = "SELECT * FROM GarageVisits";

                }
                else
                {
                    sql = $"SELECT * FROM GarageVisits WHERE GarageId = {id}";
                }
                var GarageVisits = await _context.GarageVisits.FromSqlRaw(sql).ToListAsync();
                var visitDTOs = GarageVisits
                    .Select(async v => new GarageVisitDTO
                    {
                        VisitId = v.VisitId,
                        CustomerId = v.CustomerId,
                        CustomerName = await _context.Customers.FromSqlRaw(sql_customer, v.CustomerId).Select(c => c.FirstName).FirstOrDefaultAsync(),
                        GarageId = v.GarageId,
                        GarageName = await _context.Garages.FromSqlRaw(sql_garage, v.GarageId).Select(c => c.GarageName).FirstOrDefaultAsync(),
                        VisitDate = v.VisitDate,
                        ServiceDescription = v.ServiceDescription,
                        TotalCost = v.TotalCost
                        //CanEdit = await _context.GaragePermissions.FromSqlRaw(sql_permission, userid, v.GarageId).Select(c => c.CanEdit).FirstOrDefaultAsync()
                    }).Select(t => t.Result).ToList();
                if (id != null)
                {

                    var caneditSql = $"SELECT * FROM [GaragePermissions] WHERE UserId = {userid} AND GarageId = {id}";
                    var canedit = await _context.GaragePermissions.FromSqlInterpolated($"SELECT * FROM [GaragePermissions] WHERE UserId = {userid} AND GarageId = {id}").FirstOrDefaultAsync();

                    if (canedit.CanEdit == true)
                    {
                        ViewBag.ifCanEdit = true;
                    }
                    else
                    {
                        ViewBag.ifCanEdit = false;
                    }
                    if (canedit.CanView == true)
                    {
                        ViewBag.ifCanView = true;
                    }
                    else
                    {
                        ViewBag.ifCanView = false;
                    }
                }
                else
                {
                    ViewBag.ifCanView = false;
                    ViewBag.ifCanEdit = false;
                }

                ViewBag.ActivateLayout = 0;
                return View(visitDTOs);
            }
            catch (Exception ex)
            {
                ViewBag.ActivateLayout = 2;
                return View("Error", ex);
            }
        }



        // GET: GarageVisits/Details/5
        public async Task<IActionResult> Details(int userid, int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var garageVisit = await _context.GarageVisits
                   .FromSqlRaw("SELECT TOP 1 * FROM GarageVisits WHERE VisitId = {0}", id)
                   .FirstOrDefaultAsync();
                if (garageVisit == null)
                {
                    return NotFound();
                }
                var garageVisitDTO = new GarageVisitDTO
                {
                    VisitId = garageVisit.VisitId,
                    CustomerId = garageVisit.CustomerId,
                    CustomerName = await _context.Customers.FromSqlRaw(sql_customer, garageVisit.CustomerId).Select(c => c.FirstName).FirstOrDefaultAsync(),
                    GarageId = garageVisit.GarageId,
                    GarageName = await _context.Garages.FromSqlRaw(sql_garage, garageVisit.GarageId).Select(c => c.GarageName).FirstOrDefaultAsync(),
                    VisitDate = garageVisit.VisitDate,
                    ServiceDescription = garageVisit.ServiceDescription,
                    TotalCost = garageVisit.TotalCost
                };
                ViewBag.ActivateLayout = 0;
                return View(garageVisitDTO);
            }
            catch (Exception ex)
            {
                ViewBag.ActivateLayout = 2;
                return View("Error", ex);
            }
        }

        // GET: GarageVisits/Create
        public IActionResult Create(int userid, int? id)
        {
            if (id == null)
            {
                ViewData["GarageId"] = new SelectList(_context.Garages, "GarageId", "GarageName");
            }
            else
            {
                var garage = _context.Garages.FirstOrDefault(c => c.GarageId == id);

                if (garage != null)
                {
                    ViewData["GarageId"] = garage.GarageName;
                }

            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "FirstName");
            ViewBag.ActivateLayout = 0;
            return View();
        }

        // POST: GarageVisits/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int userid, int? id, [Bind("VisitId,CustomerId,CustomerName,GarageId,GarageName,VisitDate,ServiceDescription,TotalCost,CanEdit")] GarageVisitDTO garageVisitDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var maxVisitId = await _context.GarageVisits.MaxAsync(v => (int?)v.VisitId) ?? 0;
                    var newVisitId = maxVisitId + 1;
                    var sql = $"INSERT INTO [GarageVisits] (VisitId,CustomerId,GarageId,VisitDate,ServiceDescription,TotalCost) VALUES ({newVisitId},{garageVisitDTO.CustomerId}, {garageVisitDTO.GarageId},'{garageVisitDTO.VisitDate}', '{garageVisitDTO.ServiceDescription}',{garageVisitDTO.TotalCost})";
                    await _context.Database.ExecuteSqlRawAsync(sql);

                    return Redirect($"/GarageVisits/Index?userid={userid}&id={garageVisitDTO.GarageId}");
                }
                ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "FirstName", garageVisitDTO.CustomerId);
                ViewData["GarageId"] = new SelectList(_context.Garages, "GarageId", "GarageName", garageVisitDTO.GarageId);
                ViewBag.ActivateLayout = 0;
                return View(garageVisitDTO);
            }
            catch (Exception ex)
            {
                return View("error", ex);

            }
        }

        // GET: GarageVisits/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }
                var sql = $"SELECT * FROM [GarageVisits] WHERE VisitId = {id}";
                var garageVisit = await _context.GarageVisits.FromSqlRaw(sql).FirstOrDefaultAsync();
                if (garageVisit == null)
                {
                    return NotFound();
                }
                var garageVisitDTO = new GarageVisitDTO
                {
                    VisitId = garageVisit.VisitId,
                    CustomerId = garageVisit.CustomerId,
                    CustomerName = await _context.Customers.FromSqlRaw(sql_customer, garageVisit.CustomerId).Select(c => c.FirstName).FirstOrDefaultAsync(),
                    GarageId = garageVisit.GarageId,
                    GarageName = await _context.Garages.FromSqlRaw(sql_garage, garageVisit.GarageId).Select(c => c.GarageName).FirstOrDefaultAsync(),
                    VisitDate = garageVisit.VisitDate,
                    ServiceDescription = garageVisit.ServiceDescription,
                    TotalCost = garageVisit.TotalCost
                };
                ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "FirstName", garageVisit.CustomerId);
                ViewData["GarageId"] = new SelectList(_context.Garages, "GarageId", "GarageName", garageVisit.GarageId);
                ViewBag.ActivateLayout = 0;
                return View(garageVisitDTO);
            }
            catch (Exception ex)
            {
                ViewBag.ActivateLayout = 2;
                return View("Error", ex);
            }
        }

        // POST: GarageVisits/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int userid, int id, [Bind("VisitId,CustomerId,CustomerName,GarageId,GarageName,VisitDate,ServiceDescription,TotalCost")] GarageVisitDTO garageVisitDTO)
        {
            try
            {
                //if (id != garageVisit.VisitId)
                //{
                //    return NotFound();
                //}

                if (ModelState.IsValid)
                {
                    var sql = $"UPDATE [GarageVisits] SET CustomerId = {garageVisitDTO.CustomerId}, GarageId = {garageVisitDTO.GarageId},VisitDate = '{garageVisitDTO.VisitDate}' ,ServiceDescription = '{garageVisitDTO.ServiceDescription}', TotalCost = {garageVisitDTO.TotalCost} WHERE VisitId = {garageVisitDTO.VisitId}";
                    await _context.Database.ExecuteSqlRawAsync(sql);

                    return Redirect($"/GarageVisits/Index?userid={userid}&id={garageVisitDTO.GarageId}");
                }
                ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "FirstName", garageVisitDTO.CustomerId);
                ViewData["GarageId"] = new SelectList(_context.Garages, "GarageId", "GarageName", garageVisitDTO.GarageId);
                ViewBag.ActivateLayout = 0;
                return View(garageVisitDTO);
            }
            catch
            {
                ViewBag.ActivateLayout = 2;
                return View("Error");
            }
        }

        // GET: GarageVisits/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var garageVisit = await _context.GarageVisits.FindAsync(id);

                if (garageVisit == null)
                {
                    return NotFound();
                }
                var garageVisitDTO = new GarageVisitDTO
                {
                    VisitId = garageVisit.VisitId,
                    CustomerId = garageVisit.CustomerId,
                    CustomerName = await _context.Customers.FromSqlRaw(sql_customer, garageVisit.CustomerId).Select(c => c.FirstName).FirstOrDefaultAsync(),
                    GarageId = garageVisit.GarageId,
                    GarageName = await _context.Garages.FromSqlRaw(sql_garage, garageVisit.GarageId).Select(c => c.GarageName).FirstOrDefaultAsync(),
                    VisitDate = garageVisit.VisitDate,
                    ServiceDescription = garageVisit.ServiceDescription,
                    TotalCost = garageVisit.TotalCost
                };
                ViewBag.ActivateLayout = 0;
                return View(garageVisitDTO);
            }
            catch (Exception ex)
            {
                ViewBag.ActivateLayout = 2;
                return View("Error", ex);
            }
        }


        // POST: GarageVisits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int userid, int id)
        {

            try
            {
                if(!GarageVisitExists(id)) 
                { 
                    return NotFound();
                }

                await _context.Database.ExecuteSqlRawAsync($"DELETE FROM GarageVisits WHERE VisitId = {id}");
                if (GarageVisitExists(id))
                {
                    return NotFound();
                }
                return Redirect($"/Home/Index?userid={userid}");
            }
            catch (Exception ex)
            {
                ViewBag.ActivateLayout = 2;
                return View("Error", ex);
            }
        }

        private bool GarageVisitExists(int id)
        {
            return _context.GarageVisits.Any(e => e.VisitId == id);
        }
    }
}
