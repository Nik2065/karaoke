using KaraokeWebApp3.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace KaraokeWebApp3.Controllers
{
	public class DataController : Controller
	{
		private readonly BookingService _bookingService;


		public DataController(BookingService bookingService)
		{
			_bookingService = bookingService;
		}


		[HttpPost]
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


		[HttpGet]
		public IActionResult GetBookForSpace(string spaceId)
		{
			var result = new GetBookForSpaceResponse { Success = true };

			try
			{
				var o = new SearchOptions {
					TimeType = TimeType.Future,
					SpaceId = int.Parse(spaceId)
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
}
