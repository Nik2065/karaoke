using KaraokeWebApp3.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text.Json.Serialization;

namespace KaraokeWebApp3.Controllers
{
	/// <summary>
	/// Контроллер для обработки запросов из js
	/// </summary>
	public class DataController : Controller
	{
		private readonly BookingService _bookingService;
		private int _futureBookingPeriod;
		private int _maxDurationInHours;


		public DataController(BookingService bookingService)
		{
			var configuration = new ConfigurationBuilder()
			.AddJsonFile("appsettings.json")
			.Build();
			
			_bookingService = bookingService;
			_futureBookingPeriod = int.Parse(configuration.GetSection("Settings")?.GetSection("FutureBookingPeriod")?.Value);
			_maxDurationInHours = int.Parse(configuration.GetSection("Settings")?.GetSection("MaxDurationInHours")?.Value);
			bookingService._maxDurationInHours = _maxDurationInHours;
			bookingService._futureBookingPeriod = _futureBookingPeriod;
		}


		[HttpPost]
		[Authorize]
		public IActionResult DeleteBookItem(int id)
		{
			var result = new DeleteBookItemResponse();
			try
			{
				_bookingService.DeleteBookItem(id);
				result.Success = true;
			}
			catch (Exception ex)
			{
				result.Success = false;
			}


			return Json(result);
		}

		[HttpPost]
		[Authorize]
		public IActionResult CreateBookItem([FromBody] CreateBookItemRequest request)
		{
			var result = new CreateBookItemResponse { Success = true, Message = "Бронирование создано"};
			try
			{
				//if(request.)

				//Узнаем id пользователя из сессии
				var claims = User.Claims;
				var userIdStr = claims?.FirstOrDefault(x => x.Type.Contains("nameidentifier"))?.Value;
				var userId = int.Parse(userIdStr);

				var b = DateTime.ParseExact(request.Date, "yyyy-MM-dd", CultureInfo.InvariantCulture);

				if (b < DateTime.Now)
					throw new CheckException("Время начала брониварония не может быть в прошлом периоде");

				DateTime b1 = new DateTime(b.Year, b.Month, b.Day, int.Parse(request.Begintime), 0, 0);

				var e = DateTime.ParseExact(request.Date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
				DateTime e1 = new DateTime(e.Year, e.Month, e.Day, int.Parse(request.Endtime), 0, 0);


				_bookingService.CheckBookParams(userId, b1, e1, int.Parse(request.SpaceId));
				_bookingService.CreateBookByClient(userId, b1, e1, int.Parse(request.SpaceId));
				result.Success = true;
			}
			catch(CheckException ex)
			{
				result.Success = false;
				result.Message = "Ошибка при создании бронирования:" + ex.Message;
			}
			catch (Exception ex)
			{
				result.Success = false;
				result.Message = "Ошибка при создании бронирования:" + ex.Message;
			}


			return Json(result);
		}

		/// <summary>
		/// Получить список бронирований на ближайшие 2 недели для указанного зала
		/// </summary>
		/// <param name="spaceId"></param>
		/// <returns></returns>
		[HttpGet]
		public IActionResult GetBookForSpace(string spaceId)
		{
			var result = new GetBookForSpaceResponse { Success = true };

			try
			{
				var o = new SearchOptions {
					//TimeType = TimeType.Future,
					SpaceId = int.Parse(spaceId),
					BeginPeriod = DateTime.Now,
					EndPeriod = DateTime.Now.AddDays(_futureBookingPeriod),

				};

				var list = _bookingService.GetBookings(o);


				result.BookItems = new List<BookItemDto>();
				result.BookItems = list;

			}
			catch(Exception ex)
			{
				result.Success = false;
			}
			return Json(result);
		}


		/// <summary>
		/// Получить список бронирований на ближайшие 2 недели
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public IActionResult GetBookings(string spaceId, string? date)
		{
			var result = new GetBookingsResponse { Success = true };

			try
			{
				var canParseSpaceId = Int32.TryParse(spaceId, out int sid);
				var canParseDate = DateTime.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt);
				//var b = DateTime.ParseExact(begin, "yyyy-MM-dd", CultureInfo.InvariantCulture);

				var o = new SearchOptions
				{
					BeginPeriod = DateTime.Now.Date,
					EndPeriod = DateTime.Now.AddDays(_futureBookingPeriod),

				};

				if (canParseSpaceId)
					o.SpaceId = sid;

				if (canParseDate) {
					o.BeginPeriod = new DateTime(dt.Date.Year, dt.Date.Month, dt.Date.Day);
					var v = dt.AddDays(1);
					o.EndPeriod = new DateTime(v.Date.Year, v.Date.Month, v.Date.Day);
				}


				result.BookItems = _bookingService.GetBookings(o);
				result.NotBookItems = _bookingService.GetNotBookings(result.BookItems, sid);
			}
			catch (Exception ex)
			{
				result.Success = false;
			}
			return Json(result);
		}


		/*[HttpGet]
		[Authorize]
		public IActionResult CheckBookParams(string spaceId, string begin, string end)
		{
			var result = new CheckBookParamsResponse { Success = true };

			try
			{
				var b = DateTime.ParseExact(begin, "yyyy-MM-dd", CultureInfo.InvariantCulture);
				var e = DateTime.ParseExact(end, "yyyy-MM-dd", CultureInfo.InvariantCulture);

				_bookingService.CheckBookParams(b, e, int.Parse(spaceId));
			}
			catch(CheckException ex)
			{
				result.Success=false;
				result.Msg = ex.Message;
			}
			catch (Exception ex)
			{
				//result.
			}


			return Json(result);
		}*/

	}

	public class GetBookForSpaceResponse
	{
		//public List<BookItemDto> BookItems { get; set; }
		public List<BookItemDto> BookItems { get; set; }
		public bool Success { get; set; }
	}

	public class GetBookingsResponse
	{
		//public List<BookItemDto> BookItems { get; set; }
		public List<BookItemDto> BookItems { get; set; }
		public List<NotBookItemDto> NotBookItems { get; set; }
		public bool Success { get; set; }
	}


	public class CheckBookParamsResponse
	{
		public bool CanUseThisParams { get; set; }
		public bool Success { get; set; }
		public string Msg { get; set; }

	}

	public class DeleteBookItemResponse
	{
		public bool Success { get; set; }
	}

	public class AppSettings
	{
		public int FutureBookingPeriod {  get; set; }
		public int MaxDurationInHours { get; set; }
	}

	public class CreateBookItemResponse
	{
		public bool Success { get; set; }
		public string Message { get; set; }
	}

	public class CreateBookItemRequest
	{
		[JsonPropertyName("Date")]
		public string Date { get; set; } = "";

		[JsonPropertyName("Begintime")]
		public string Begintime { get; set; } = "";

		[JsonPropertyName("Endtime")]
		public string Endtime { get; set; } = "";

		[JsonPropertyName("SpaceId")]
		public string SpaceId { get; set; } = "";
	}
}
