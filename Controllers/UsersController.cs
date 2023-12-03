using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Garage_Management.Models;
using Garage_Management.DTO;
using Microsoft.Data.SqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Garage_Management.Controllers
{
    public class UsersController : Controller
    {
        private readonly Garage_ManagementContext _context;

        public UsersController(Garage_ManagementContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index(int? userid)
        {
           
            try
            {
                string sql;
                if (userid == 0)
                {
                    sql = "SELECT * FROM [Users]";
                }
                else
                {
                    sql = $"SELECT * FROM [Users] WHERE UserId= {userid}";
                }
                var users = await _context.Users.FromSqlRaw(sql).ToListAsync();
                var userDTOs = users
                        .Select(u => new UserDTO
                        {
                            UserId = u.UserId,
                            Username = u.Username,
                            Password = u.Password
                        }).ToList();

                ViewBag.ActivateLayout = 0;
                return View(userDTOs);
            }
            catch
            {
                ViewBag.ActivateLayout = 2;
                return View("Error");
            }

        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int userid, int? id)
        {
            try
            {
                int thisid = userid;
                if (userid == 0)
                {
                    thisid = id.Value;
                }
                if (thisid == null)
                {
                    return NotFound();
                }

                var user = await _context.Users
                .FromSqlRaw("SELECT TOP 1 * FROM Users WHERE UserId = {0}", thisid)
                .FirstOrDefaultAsync();

                if (user == null)
                {
                    return NotFound();
                }

                var userDTO = new UserDTO
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Password = user.Password
                };

                ViewBag.ActivateLayout = 0;
                return View(userDTO);
            }
            catch
            {
                ViewBag.ActivateLayout = 2;
                return View("Error");
            }
        }


        // GET: Users/SignUp
        public IActionResult SignUp()
        {
            ViewBag.ActivateLayout = 2;
            return View();
        }

        // POST: Users/SignUp
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp([Bind("UserId,Username,Password")] UserDTO userDTO)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    var sql = "SELECT UserId, Username, Password FROM [Users] WHERE Username = {0} AND Password = {1}";

                    //var usernameParameter = new SqlParameter("@Username", userDTO.Username);
                    //var passwordParameter = new SqlParameter("@Password", userDTO.Password);

                    var user = await _context.Users.FromSqlRaw(sql, userDTO.Username, userDTO.Password)
                        .FirstOrDefaultAsync();

                    //var user = await _context.Users.FindAsync(id);
                    if (user == null)
                    {
                        ViewBag.ActivateLayout = 2;
                        ViewBag.ErrorMessage = "User does not exist";
                        return View(userDTO);
                    }

                    return Redirect($"/Home/Index?userid={user.UserId}");

                }
                ViewBag.ActivateLayout = 2;
                return View(userDTO);
            }
            catch 
            {
                ViewBag.ActivateLayout = 2;
                return View("Error");
            }
        }


        // GET: Users/Create
        public IActionResult Create()
        {
            ViewBag.ActivateLayout = 0;
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int userid, [Bind("UserId,Username,Password")] UserDTO userDTO)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    var u = await _context.Users
                .FromSqlRaw("SELECT TOP 1 * FROM Users WHERE Username = {0}", userDTO.Username)
                .FirstOrDefaultAsync();
                    if (u != null)
                    {
                        ViewBag.ActivateLayout = 0;
                        ViewBag.ErrorMessage = "User name exist";
                        return View(userDTO);
                    }
                    var maxUserId = await _context.Users.MaxAsync(u => (int?)u.UserId) ?? 0;
                    var newUserId = maxUserId + 1;
                    var sql = $"INSERT INTO [Users] (UserId, Username, Password) VALUES ({newUserId}, '{userDTO.Username}', '{userDTO.Password}')";

                    await _context.Database.ExecuteSqlRawAsync(sql);

                    var maxPermissionId = await _context.Permissions.MaxAsync(m => (int?)m.PermissionId) ?? 0;
                    
                    var customers = await _context.Customers.ToListAsync();
                    foreach (var customer in customers)
                    {
                         maxPermissionId = maxPermissionId + 1;
                        var negativePermission = new Permission
                        {
                            PermissionId = maxPermissionId,
                            UserId = newUserId,
                            CustomerId = customer.CustomerId,
                            CanView = false,
                            CanEdit = false
                        };
                        _context.Permissions.Add(negativePermission);

                    }

                    var maxG_PermissionId = await _context.Permissions.MaxAsync(m => (int?)m.PermissionId) ?? 0;

                    var garages = await _context.Garages.ToListAsync();
                    foreach (var garage in garages)
                    {
                        maxG_PermissionId = maxG_PermissionId + 1;
                        var gPermission = new GaragePermission
                        {
                            PermissionId = maxG_PermissionId,
                            UserId = newUserId,
                            GarageId = garage.GarageId,
                            CanView = false,
                            CanEdit = false
                        };
                        _context.GaragePermissions.Add(gPermission);
                    }
                        await _context.SaveChangesAsync();

                    return Redirect($"/Users/Index?userid={userid}");
                    //return RedirectToAction(nameof(Index));
                }

                ViewBag.ActivateLayout = 0;
                return View(userDTO);
            }
            catch
            {
                ViewBag.ActivateLayout = 2;
                return View("Error");
            }
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }
                var sql = $"SELECT UserId, Username, Password FROM [Users] WHERE UserId = {id}";
                var user = await _context.Users.FromSqlRaw(sql).FirstOrDefaultAsync();

                if (user == null)
                {
                    return NotFound();
                }

                var userDTO = new UserDTO
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Password = user.Password
                };

                ViewBag.ActivateLayout = 0;
                return View(userDTO);
            }
            catch
            {
                ViewBag.ActivateLayout = 2;
                return View("Error");
            }
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int userid,int id, [Bind("UserId,Username,Password")] UserDTO userDTO)
        {
            try
            {
                //if (id != userDTO.UserId)
                //{
                //    return NotFound();
                //}

                if (ModelState.IsValid)
                {
                    var sql = $"UPDATE [Users] SET Username = '{userDTO.Username}', Password = '{userDTO.Password}' WHERE UserId = {userDTO.UserId}";
                    await _context.Database.ExecuteSqlRawAsync(sql);
                    return Redirect($"/Users/Index?userid={userid}");
                   //return RedirectToAction(nameof(Index));
                }

                ViewBag.ActivateLayout = 0;
                return View(userDTO);
            }
            catch
            {
                ViewBag.ActivateLayout = 2;
                return View("Error");
            }
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null )
                {
                    return NotFound();
                }

                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                var userDTO = new UserDTO
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Password = user.Password
                };

                ViewBag.ActivateLayout = 0;
                return View(userDTO);
            }
            catch
            {
                ViewBag.ActivateLayout = 2;
                return View("Error");
            }
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int userid, int id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }


                var sql = $"DELETE [Users] WHERE UserId = {id}";
                await _context.Database.ExecuteSqlRawAsync(sql);
                if (UserExists(id))
                {
                    ViewBag.ActivateLayout = 2;
                    return View("Error");
                }
                return Redirect($"/Users/Index?userid={0}");
            }
            catch
            {
                ViewBag.ActivateLayout = 2;
                return View("Error");
            }
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
