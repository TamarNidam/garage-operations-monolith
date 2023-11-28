using Garage_Management.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Garage_Management.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            ViewBag.ActivateLayout = true;
            return View();
        }
        public IActionResult Index1()
        {
            ViewBag.ActivateLayout = true;
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
