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
        public async Task<IActionResult> Index()
        {
            try
            {
               
                var sql = "SELECT * FROM [Users]";
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
                ViewBag.ActivateLayout = 0;
                return View("Error");
            }

        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? userid, int? id)
        {
            var thisid =userid;
            if(userid == 0)
            {
                thisid = id;
            }
           

            try
            {
                if (thisid == null)
                {
                    return NotFound();
                }

                var user = await _context.Users
                    .FirstOrDefaultAsync(m => m.UserId == thisid);

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
                ViewBag.ActivateLayout = 1;
                if (thisid == 0)
                {
ViewBag.ActivateLayout = 0;
                }
                    
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
                    var sql = "SELECT UserId, Username, Password FROM [Users] WHERE Username = @Username AND Password = @Password";

                    var usernameParameter = new SqlParameter("@Username", userDTO.Username);
                    var passwordParameter = new SqlParameter("@Password", userDTO.Password);

                    var user = await _context.Users.FromSqlRaw(sql, usernameParameter, passwordParameter)
                        .FirstOrDefaultAsync();

                    //var user = await _context.Users.FindAsync(id);
                    if (user == null)
                    {
                        ViewBag.ActivateLayout = 2;
                        ViewBag.ErrorMessage = "User does not exist";
                        return View(userDTO);
                    }
                    
                    return Redirect($"{user.UserId}/Home/Index");

                }
                ViewBag.ActivateLayout = 2;
                return View(userDTO);
            }
            catch (Exception ex)
            {
                ViewBag.ActivateLayout = 2;
                return RedirectToAction(nameof(Error));
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
        public async Task<IActionResult> Create([Bind("UserId,Username,Password")] UserDTO userDTO)
        {
            try
            {
                
                if (ModelState.IsValid)
                {
                    var maxUserId = await _context.Users.MaxAsync(u => (int?)u.UserId) ?? 0;
                    var newUserId = maxUserId + 1;
                    var sql = $"INSERT INTO [Users] (UserId, Username, Password) VALUES ({newUserId}, '{userDTO.Username}', '{userDTO.Password}')";
                    
                    await _context.Database.ExecuteSqlRawAsync(sql);
                    
                    return RedirectToAction(nameof(Index));
                }
                ViewBag.ActivateLayout = true;
                return View(userDTO);
            }
            catch
            {
                ViewBag.ActivateLayout = true;
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

                //var user = await _context.Users.FindAsync(id);
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
                ViewBag.ActivateLayout = true;
                return View(userDTO);
            }
            catch
            {
                ViewBag.ActivateLayout = true;
                return View("Error");
            }
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,Username,Password")] UserDTO userDTO)
        {
            try
            {
                if (id != userDTO.UserId)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    var sql = $"UPDATE [Users] SET Username = '{userDTO.Username}', Password = '{userDTO.Password}' WHERE UserId = {userDTO.UserId}";
                    await _context.Database.ExecuteSqlRawAsync(sql);

                    //var user = new User
                    //{
                    //    UserId = userDTO.UserId,
                    //    Username = userDTO.Username,
                    //    Password = userDTO.Password
                    //};

                    //try
                    //{
                    //    _context.Update(user);
                    //    await _context.SaveChangesAsync();
                    //}
                    //catch (DbUpdateConcurrencyException)
                    //{
                    //    if (!UserExists(userDTO.UserId))
                    //    {
                    //        return NotFound();
                    //    }
                    //    else
                    //    {
                    //        throw;
                    //    }
                    //}
                    return RedirectToAction(nameof(Index));
                }
                ViewBag.ActivateLayout = true;
                return View(userDTO);
            }
            catch
            {
                ViewBag.ActivateLayout = true;
                return View("Error");
            }
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null)
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

                ViewBag.ActivateLayout = true;
                return View(userDTO);
            }
            catch
            {
                ViewBag.ActivateLayout = true;
                return View("Error");
            }
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user != null)
                {
                    _context.Users.Remove(user);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ViewBag.ActivateLayout = true;
                return View("Error");
            }
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
