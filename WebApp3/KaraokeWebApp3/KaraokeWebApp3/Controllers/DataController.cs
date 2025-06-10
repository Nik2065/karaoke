using KaraokeWebApp3.Dto;
using Microsoft.AspNetCore.Mvc;

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

	}

	public class GetBookForSpaceResponse
	{
		//public List<BookItemDto> BookItems { get; set; }
		public List<BookItemDto> BookItems { get; set; }
		public bool Success { get; set; }
	}




	public class DeleteBookItemResponse
	{
		public bool Success { get; set; }
	}
}
