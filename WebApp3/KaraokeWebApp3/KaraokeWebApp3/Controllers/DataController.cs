using KaraokeWebApp3.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

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

		[HttpGet]
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
		}

	}

	public class GetBookForSpaceResponse
	{
		//public List<BookItemDto> BookItems { get; set; }
		public List<BookItemDto> BookItems { get; set; }
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
}
