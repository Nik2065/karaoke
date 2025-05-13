using DataAccess;
using KaraokeWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace KaraokeWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IConfiguration conf)
        {
            _logger = logger;
            db = new DataContext(conf);
        }

        private DataContext db;
        

        public IActionResult Index()
        {
            var one = db.Users.ToList();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



        public IActionResult SignInAsClient()
        {
            return View();
        }


        public IActionResult SignInAsManager()
        {
            return View();
        }

        public IActionResult StartPage()
        {
            return View();
        }

        public IActionResult BookingPageForClient()
        {
            return View();
        }

        public IActionResult BookingPageForManager()
        {
            return View();
        }



    }
}