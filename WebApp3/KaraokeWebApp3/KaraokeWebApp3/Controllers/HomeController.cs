using KaraokeWebApp3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Globalization;

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
            so.TimeType = TimeType.Future;
            model.FutureBookings = _bookingService.GetBookings(so);
			so.TimeType = TimeType.Past;
			model.PastBookings = _bookingService.GetBookings(so);


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

			//
			// todo
			// проверка полученных значений на разумность 

			var model = new CreateBookForClientResultModel();
			model.Success = true;
            model.Message = "Бронирование создано";

			try
            {
                var b = DateTime.ParseExact(begin, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime b1 = new DateTime(b.Year, b.Month, b.Day, begintime, 0, 0);

                var e = DateTime.ParseExact(end, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime e1 = new DateTime(e.Year, e.Month, e.Day, endtime, 0, 0);

                _bookingService.CreateBookByClient(userId, b1, e1, zalId);
            }
            catch (Exception ex)
            {
				model.Success = false;
				model.Message = "Ошибка при создании бронирования:" + ex.Message;
			}

            


			return View(model);
		}

		
		[Authorize]
		[HttpPost]
		public IActionResult CreateBookForManagerResult(int clientId, int zalId, string begin, int begintime, string end, int endtime)
		{
			//var model = new CreateBookForClientModel();
			//model.Spaces = _bookingService.GetSpaces();
			//пишем бронь в базу
			//Узнаем id пользователя из сессии
			var claims = User.Claims;
			var userIdStr = claims?.FirstOrDefault(x => x.Type.Contains("nameidentifier"))?.Value;
			var userId = int.Parse(userIdStr);

			//
			// todo
			// проверка полученных значений на разумность 

			var model = new CreateBookForManagerResultModel();
			model.Success = true;
			model.Message = "Бронирование создано";

			try
			{
				var b = DateTime.ParseExact(begin, "yyyy-MM-dd", CultureInfo.InvariantCulture);
				DateTime b1 = new DateTime(b.Year, b.Month, b.Day, begintime, 0, 0);

				var e = DateTime.ParseExact(end, "yyyy-MM-dd", CultureInfo.InvariantCulture);
				DateTime e1 = new DateTime(e.Year, e.Month, e.Day, endtime, 0, 0);

				_bookingService.CreateBookByManager(clientId, userId, b1, e1, zalId);
			}
			catch (Exception ex)
			{
				model.Success = false;
				model.Message = "Ошибка при создании бронирования:" + ex.Message;
			}




			return View(model);
		}


        /// <summary>
        /// Страница "Залы"
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Spaces()
        {
            return View();
        }



        /// <summary>
        /// Страница услуг
        /// </summary>
        /// <returns></returns>
		[HttpGet]
		public IActionResult Services()
		{
			return View();
		}

	}
}