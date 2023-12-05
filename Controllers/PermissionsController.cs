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


        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }
                var sql = $"SELECT * FROM [Permissions] WHERE PermissionId = {id}";
                var cpermission = await _context.Permissions.FromSqlRaw(sql).FirstOrDefaultAsync();
                if (cpermission == null)
                {
                    return NotFound();
                }
                var permission = new CustomerPermissionDTO
                {
                    PermissionId = cpermission.PermissionId,
                    UserId = cpermission.UserId,
                    UserName = await _context.Users.FromSqlRaw(sql_user, cpermission.UserId).Select(c => c.Username).FirstOrDefaultAsync(),
                    CustomerId = cpermission.CustomerId,
                    CustomerName = await _context.Customers.FromSqlRaw(sql_customer, cpermission.CustomerId).Select(c => c.FirstName).FirstOrDefaultAsync(),
                    CanView = cpermission.CanView,
                    CanEdit = cpermission.CanEdit
                };
                ViewBag.ActivateLayout = 0;
                return View(permission);
            }
            catch (Exception ex)
            {
                ViewBag.ActivateLayout = 2;
                return View("Error", ex);
            }
        }

        // POST: Permissions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int userid, int id, [Bind("PermissionId,UserId,CustomerId,CanView,CanEdit")] CustomerPermissionDTO customerPermissionDTO)
        {
            try
            {
                //if (id != permission.PermissionId)
                //{
                //    return NotFound();
                //}

                if (ModelState.IsValid)
                {
                    var sql = $"UPDATE [Permissions] SET CanView = '{customerPermissionDTO.CanView}', CanEdit = '{customerPermissionDTO.CanEdit}' WHERE PermissionId = {customerPermissionDTO.PermissionId}";
                    await _context.Database.ExecuteSqlRawAsync(sql);

                    return Redirect($"/Permissions/Index?userid=0&id={customerPermissionDTO.UserId}");

                }
                ViewBag.ActivateLayout = 0;
                return View(customerPermissionDTO);
            }
            catch (Exception ex)
            {
                ViewBag.ActivateLayout = 2;
                return View("Error", ex);
            }
        }


        private bool PermissionExists(int id)
        {
            return _context.Permissions.Any(e => e.PermissionId == id);
        }
    }
}
