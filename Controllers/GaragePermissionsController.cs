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
    public class GaragePermissionsController : Controller
    {
        private const string sql_garage = "SELECT * FROM Garage WHERE GarageId = {0}";
        private const string sql_user = "SELECT * FROM Users WHERE UserId = {0}";
                private readonly Garage_ManagementContext _context;

        public GaragePermissionsController(Garage_ManagementContext context)
        {
            _context = context;
        }

        // GET: GaragePermissions
        public async Task<IActionResult> Index(int userid, int id)
        {
            try
            {
                string sql = $"SELECT * FROM GaragePermissions WHERE UserId = {id}";

                var permissions = await _context.GaragePermissions.FromSqlRaw(sql).ToListAsync();
                var permissionsDTOs = permissions
                    .Select(async p => new GaragePermissionDTO
                    {
                        PermissionId = p.PermissionId,
                        UserId = p.UserId,
                        UserName = await _context.Users.FromSqlRaw(sql_user, p.UserId).Select(c => c.Username).FirstOrDefaultAsync(),
                        GarageId = p.GarageId,
                        GarageName = await _context.Garages.FromSqlRaw(sql_garage, p.GarageId).Select(c => c.GarageName).FirstOrDefaultAsync(),
                        CanView = p.CanView,
                        CanEdit = p.CanEdit
                    }).Select(t => t.Result).ToList();

                ViewBag.ActivateLayout = 0;
                 return View(permissionsDTOs);
            }
            catch (Exception ex)
            {
                ViewBag.ActivateLayout = 2;
                return View("Error", ex);
            }
        }

        //// GET: GaragePermissions/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var garagePermission = await _context.GaragePermissions
        //        .Include(g => g.Garage)
        //        .Include(g => g.User)
        //        .FirstOrDefaultAsync(m => m.PermissionId == id);
        //    if (garagePermission == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(garagePermission);
        //}

        //// GET: GaragePermissions/Create
        //public IActionResult Create()
        //{
        //    ViewData["GarageId"] = new SelectList(_context.Garages, "GarageId", "GarageName");
        //    ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Password");
        //    return View();
        //}

        //// POST: GaragePermissions/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("PermissionId,UserId,GarageId,CanView,CanEdit")] GaragePermission garagePermission)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(garagePermission);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["GarageId"] = new SelectList(_context.Garages, "GarageId", "GarageName", garagePermission.GarageId);
        //    ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Password", garagePermission.UserId);
        //    return View(garagePermission);
        //}

        // GET: GaragePermissions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }
                var sql = $"SELECT * FROM [GaragePermissions] WHERE PermissionId = {id}";
                var garagePermission = await _context.GaragePermissions.FromSqlRaw(sql).FirstOrDefaultAsync();
                if (garagePermission == null)
                {
                    return NotFound();
                }
                var permission = new GaragePermissionDTO
                {
                    PermissionId = garagePermission.PermissionId,
                    UserId = garagePermission.UserId,
                    UserName = await _context.Users.FromSqlRaw(sql_user, garagePermission.UserId).Select(c => c.Username).FirstOrDefaultAsync(),
                    GarageId = garagePermission.GarageId,
                    GarageName = await _context.Garages.FromSqlRaw(sql_garage, garagePermission.GarageId).Select(c => c.GarageName).FirstOrDefaultAsync(),
                    CanView = garagePermission.CanView,
                    CanEdit = garagePermission.CanEdit
                };
                //ViewData["CAN"] = new SelectList(_context.Garages, "GarageId", "GarageName", garagePermission.GarageId);
                ViewBag.ActivateLayout = 0;
                return View(permission);
            }
            catch (Exception ex)
            {
                ViewBag.ActivateLayout = 2;
                return View("Error", ex);
            }
        }

        // POST: GaragePermissions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int userid, int id, [Bind("PermissionId,UserId,GarageId,CanView,CanEdit")] GaragePermissionDTO garagePermissionDTO)
        {
            try
            {
                //if (id != garagePermission.PermissionId)
                //{
                //    return NotFound();
                //}

                if (ModelState.IsValid)
                {
                    var sql = $"UPDATE [GaragePermissions] SET CanView = '{garagePermissionDTO.CanView}', CanEdit = '{garagePermissionDTO.CanEdit}' WHERE PermissionId = {garagePermissionDTO.PermissionId}";
                    await _context.Database.ExecuteSqlRawAsync(sql);

                    return Redirect($"/GaragePermissions/Index?userid=0&id={garagePermissionDTO.UserId}");

                }
                ViewBag.ActivateLayout = 0;
                return View(garagePermissionDTO);
            }
            catch(Exception ex)
            {
                ViewBag.ActivateLayout = 2;
                return View("Error",ex);
            }
        }

        //// GET: GaragePermissions/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var garagePermission = await _context.GaragePermissions
        //        .Include(g => g.Garage)
        //        .Include(g => g.User)
        //        .FirstOrDefaultAsync(m => m.PermissionId == id);
        //    if (garagePermission == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(garagePermission);
        //}

        //// POST: GaragePermissions/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var garagePermission = await _context.GaragePermissions.FindAsync(id);
        //    if (garagePermission != null)
        //    {
        //        _context.GaragePermissions.Remove(garagePermission);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        private bool GaragePermissionExists(int id)
        {
            return _context.GaragePermissions.Any(e => e.PermissionId == id);
        }
    }
}
