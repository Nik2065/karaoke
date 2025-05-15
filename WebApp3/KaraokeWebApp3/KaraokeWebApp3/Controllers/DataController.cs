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
	}

	public class DeleteBookItemResponse
	{
		public bool Success { get; set; }
	}
}
