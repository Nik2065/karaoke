using KaraokeWebApp3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace KaraokeWebApp3.Controllers
{
    
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BookingService _bookingService;

		public HomeController(ILogger<HomeController> logger, BookingService bookingService)
        {
            _logger = logger;
            _bookingService = bookingService;
        }

        public IActionResult Index()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [Authorize]
        [HttpGet]
        public IActionResult HomeForClient()
        {
            //Узнаем id пользователя из сессии
            var claims = User.Claims;
            var userIdStr = claims?.FirstOrDefault(x=>x.Type.Contains("nameidentifier"))?.Value;
            var userId = int.Parse(userIdStr);

            //получаем записи из базы
            var so = new SearchOptions { UserId = userId };
			var list = _bookingService.GetBookings(so);

            var model = new HomeForClientModel();
            model.Bookings = list;

			return View(model);
        }


        [Authorize]
        [HttpGet]
        public IActionResult HomeForManager()
        {
            //получаем записи из базы
            var so = new SearchOptions();
            var list = _bookingService.GetBookings(so);

            var model = new HomeForManagerModel();
            model.Bookings = list;
            model.Users = _bookingService.GetUsers();

			return View(model);
        }

        [Authorize]
        [HttpGet]
        public IActionResult CreateBookForManager()
        {
            var model = new CreateBookForManagerModel();
            model.Clients = _bookingService.GetClients();
            model.Spaces = _bookingService.GetSpaces();

			return View(model);
        }


        [Authorize]
        [HttpGet]
        public IActionResult CreateBookForClient()
        {
			var model = new CreateBookForClientModel();
			model.Spaces = _bookingService.GetSpaces();

			return View(model);
        }

		[Authorize]
		[HttpPost]
		public IActionResult CreateBookForClientResult(int zalId, string begin, int begintime, string end, int endtime)
		{
			//var model = new CreateBookForClientModel();
			//model.Spaces = _bookingService.GetSpaces();
			//пишем бронь в базу
			//Узнаем id пользователя из сессии
			var claims = User.Claims;
			var userIdStr = claims?.FirstOrDefault(x => x.Type.Contains("nameidentifier"))?.Value;
			var userId = int.Parse(userIdStr);

			_bookingService.BookClient(userId,)


            var model = new CreateBookForClientResultModel();


			return View(model);
		}

	}
}