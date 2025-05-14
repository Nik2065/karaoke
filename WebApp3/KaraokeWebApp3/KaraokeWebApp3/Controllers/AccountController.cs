using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KaraokeWebApp3.Controllers
{
    public class AccountController : Controller
    {
        private readonly AuthService _authService;

        public AccountController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string phone, string password)
        {
            var user = await _authService.Authenticate(phone, password);
            if (user == null) return View();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, "Client")
            };

            var identity = new ClaimsIdentity(claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));

            return RedirectToAction("HomeForClient", "Home");
        }

        [HttpGet]
        public IActionResult LoginForManager() => View();

        /// <summary>
        /// Вход для менеджера
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> LoginForManager(string username, string password)
        {
            var user = await _authService.AuthenticateManager(username, password);
            if (user == null) return View();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, "Manager")
            };

            var identity = new ClaimsIdentity(claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));

            return RedirectToAction("HomeForManager", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        //

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register() => View();


        // POST: /Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(string username, string password, string phone)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Номер телефона, имя и пароль обязательны";
                return View();
            }

            var result = await _authService.Register(username, password, phone);

            if (result)
            {
                // Автоматический вход после регистрации
                return RedirectToAction("Login");
            }

            ViewBag.Error = "Пользователь с таким логином уже существует";
            return View();
        }



        // GET: /Account/RegisterForManager
        [HttpGet]
        public IActionResult RegisterForManager() => View();


        // POST: /Account/Register
        [HttpPost]
        public async Task<IActionResult> RegisterForManager(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Логин и пароль обязательны";
                return View();
            }

            var result = await _authService.RegisterForManager(username, password);

            if (result)
            {
                // Автоматический вход после регистрации
                return RedirectToAction("LoginForManager");
            }

            ViewBag.Error = "Пользователь с таким логином уже существует";
            return View();
        }

    }
}
