using Garage_Management.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Garage_Management.DTO;
using Microsoft.EntityFrameworkCore;

namespace Garage_Management.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Garage_ManagementContext _context;

        public HomeController(Garage_ManagementContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}

        public IActionResult Index(int? userid)
        {
          
                ViewBag.ActivateLayout = 0;
            
            return View();
        }
        public async Task<IActionResult> Privacy(int userid)
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
