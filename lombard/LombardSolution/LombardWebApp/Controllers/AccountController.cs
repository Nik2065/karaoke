using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LombardWebApp.Controllers
{
	public class AccountController : Controller
	{
		    
        private readonly AuthService _authService;

		public AccountController(AuthService authService)
		{
			_authService = authService;
		}

        [HttpGet]
        public IActionResult Login(string? msg)
        {
            if (!string.IsNullOrEmpty(msg))
            {
                ViewBag.Error = msg;
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _authService.Authenticate(username, password);
            if (user == null) return Login("Ошибочный логин или пароль");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            };

            var identity = new ClaimsIdentity(claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));

            return RedirectToAction("HomeForManager", "Home");
        }


        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register() => View();


        // POST: /Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Логин и пароль обязательны";
                return View();
            }

            var result = await _authService.Register(username, password);

            if (result)
            {
                // Автоматический вход после регистрации
                return RedirectToAction("Login");
            }

            ViewBag.Error = "Пользователь с таким логином уже существует";
            return View();
        }


        //[HttpPost]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

    }
}
