using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KaraokeWebApp.Controllers
{
    public class ManagerController : Controller
    {
        
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult SignInAsManager()
        {
            return View();
        }

        

    }
}
