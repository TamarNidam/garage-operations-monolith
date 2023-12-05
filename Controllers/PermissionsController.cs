using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Garage_Management.Models;
using Garage_Management.DTO;
using System.Security;

namespace Garage_Management.Controllers
{

    public class PermissionsController : Controller
    {
        private const string sql_customer = "SELECT * FROM Customers WHERE CustomerId = {0}";
        private const string sql_user = "SELECT * FROM Users WHERE UserId = {0}";
        private readonly Garage_ManagementContext _context;

        public PermissionsController(Garage_ManagementContext context)
        {
            _context = context;
        }

        // GET: Permissions
        public async Task<IActionResult> Index(int userid, int id)
        {
            try
            {
                string sql = $"SELECT * FROM Permissions WHERE UserId = {id}";

                var permissions = await _context.Permissions.FromSqlRaw(sql).ToListAsync();
                var permissionsDTOs = permissions
                    .Select(async p => new CustomerPermissionDTO
                    {
                        PermissionId = p.PermissionId,
                        UserId = p.UserId,
                        UserName = await _context.Users.FromSqlRaw(sql_user, p.UserId).Select(c => c.Username).FirstOrDefaultAsync(),
                        CustomerId = p.CustomerId,
                        CustomerName = await _context.Customers.FromSqlRaw(sql_customer, p.CustomerId).Select(c => c.FirstName).FirstOrDefaultAsync(),
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
    

        //// GET: Permissions/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var permission = await _context.Permissions
        //        .Include(p => p.Customer)
        //        .Include(p => p.User)
        //        .FirstOrDefaultAsync(m => m.PermissionId == id);
        //    if (permission == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(permission);
        //}

        //// GET: Permissions/Create
        //public IActionResult Create()
        //{
        //    ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "FirstName");
        //    ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Password");
        //    return View();
        //}

        //// POST: Permissions/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("PermissionId,UserId,CustomerId,CanView,CanEdit")] Permission permission)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(permission);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "FirstName", permission.CustomerId);
        //    ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Password", permission.UserId);
        //    return View(permission);
        //}

        // GET: Permissions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var permission = await _context.Permissions.FindAsync(id);
            if (permission == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "FirstName", permission.CustomerId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Password", permission.UserId);
            return View(permission);
        }

        // POST: Permissions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PermissionId,UserId,CustomerId,CanView,CanEdit")] Permission permission)
        {
            if (id != permission.PermissionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(permission);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PermissionExists(permission.PermissionId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "FirstName", permission.CustomerId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Password", permission.UserId);
            return View(permission);
        }

        //// GET: Permissions/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var permission = await _context.Permissions
        //        .Include(p => p.Customer)
        //        .Include(p => p.User)
        //        .FirstOrDefaultAsync(m => m.PermissionId == id);
        //    if (permission == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(permission);
        //}

        //// POST: Permissions/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var permission = await _context.Permissions.FindAsync(id);
        //    if (permission != null)
        //    {
        //        _context.Permissions.Remove(permission);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        private bool PermissionExists(int id)
        {
            return _context.Permissions.Any(e => e.PermissionId == id);
        }
    }
}
