using KaraokeWebApp3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Globalization;
using DinkToPdf;
using DinkToPdf.Contracts;

namespace KaraokeWebApp3.Controllers
{
    
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BookingService _bookingService;
		private readonly IConverter _pdfConverter;

		//private int _futureBookingPeriod;
		//private int _maxDurationInHours;

		public HomeController(ILogger<HomeController> logger, AppDbContext db, IConverter pdfConverter)
        {
            _logger = logger;

			var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

			_bookingService = new BookingService(db);
			var _futureBookingPeriod = int.Parse(configuration.GetSection("Settings")?.GetSection("FutureBookingPeriod")?.Value);
			var _maxDurationInHours = int.Parse(configuration.GetSection("Settings")?.GetSection("MaxDurationInHours")?.Value);
			_bookingService._maxDurationInHours = _maxDurationInHours;
			_bookingService._futureBookingPeriod = _futureBookingPeriod;
			_pdfConverter = pdfConverter;

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
            //so.TimeType = TimeType.Future;
            so.BeginPeriod = DateTime.Now;
            so.EndPeriod = DateTime.Now.AddDays(_bookingService._futureBookingPeriod);
            model.FutureBookings = _bookingService.GetBookings(so);
			//so.TimeType = 
			so.BeginPeriod = DateTime.Now.AddYears(-1);
			so.EndPeriod = DateTime.Now;
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
            var so = new SearchOptions { 
                BeginPeriod = DateTime.Now.Date, 
                EndPeriod = DateTime.Now.AddDays(_bookingService._futureBookingPeriod)
            };

			model.BookItems = _bookingService.GetBookings(so);

			return View("CreateBookForClient2", model);
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

        [HttpGet]
        public IActionResult KoreanEvenings()
        {
			return View();
		}


		[HttpGet]
		public IActionResult RetroEvenings()
        {
            return View();
        }

		[HttpGet]
		public IActionResult About()
		{
			return View();
		}

		[HttpGet]
		public IActionResult Rules()
		{
			return View();
		}


		[HttpGet]
		public IActionResult Report()
		{

			var htmlContent = GenerateHtmlContent();


			var doc = new HtmlToPdfDocument()
			{
				GlobalSettings = {
				PaperSize = PaperKind.A4,
				Orientation = Orientation.Portrait,
				Margins = new MarginSettings(20, 15, 20, 15)
			},
				Objects = {
				new ObjectSettings()
				{
					HtmlContent = htmlContent,
					WebSettings = { DefaultEncoding = "utf-8" }
				}
			}
			};

			byte[] pdfBytes = _pdfConverter.Convert(doc);


			return File(pdfBytes, "application/pdf", "DinkReport.pdf");

		}

		private string GenerateHtmlContent()
		{
			// Ваша реализация генерации HTML
			return @"<html><body><h1>Работает с DinkToPdf!</h1></body></html>";
		}


		//private byte[] GeneratePdf()
		//{
		//	// Создаем PDF-документ
		//	/*string htmlCode = "<html>wooo</html>";
		//	PdfDocument pdf = PdfGenerator.GeneratePdf(htmlCode, PageSize.A4);
		//	var ms = new MemoryStream();
		//	pdf.Save(ms);

		//	return ms.ToArray();*/


		//	PdfDocument pdf = PdfGenerator.GeneratePdf("<p><h1>Hello World</h1>This is html rendered text</p>", PageSize.A4);
		//	var ms = new MemoryStream();
		//	pdf.Save(ms);
		//	return ms.ToArray();
		//}
	}
}