using Microsoft.AspNetCore.Mvc;

namespace LombardWebApp.Controllers
{
	public class DataController : Controller
	{
		private readonly MarketingService _marketingService;

		public DataController(MarketingService marketingService)
		{
			_marketingService = marketingService;
		}

		[HttpPost]
		public IActionResult SendForm([FromBody] SendFormRequest request)
		{
			
			var result = new SendFormResponse {Success = true};
			try
			{
				_marketingService.AddLead(name: request?.Name, phone: request?.Phone);
			}
			catch (Exception ex)
			{
				result.Success = false;
				result.Message = ex.Message;
			}

			return Json(result);
		}
	}

	public class SendFormResponse
	{
		public string Message { get; set; }
		public bool Success { get; set; }
	}

	public class SendFormRequest
	{
		public string? Name { get; set; }
		public string? Phone { get; set; }
	}

}