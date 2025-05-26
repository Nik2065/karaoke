using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

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



		[HttpPost]
		public IActionResult CalcDeposit([FromBody] CalcDepositRequest request)
		{

			var result = new CalcDepositResponse { Success = true };
			try
			{
				if (request.Sum == null
					|| !request.DepositType.HasValue)
				{
					result.Success = false;
					result.Message = "Не хватает данных для рассчета";
				}
				else
				{
					var r = _marketingService.CalcDeposit(type: (DepositTypeEnum)request.DepositType, sum: (decimal)request.Sum);
					result.SumInMonth = r;
				}
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

	public class CalcDepositResponse
	{
		public string Message { get; set; }
		public bool Success { get; set; }

		public decimal SumInMonth { get; set; }
	}

	public class SendFormRequest
	{
		public string? Name { get; set; }
		public string? Phone { get; set; }
	}

	public class CalcDepositRequest
	{
		[JsonPropertyName("sum")]
		public int? Sum { get; set; }

		[JsonPropertyName("depositType")]
		public int? DepositType { get; set; }
	}




}