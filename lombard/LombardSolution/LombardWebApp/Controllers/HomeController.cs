using LombardWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LombardWebApp.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly MarketingService _marketingService;

        public HomeController(ILogger<HomeController> logger, MarketingService marketingService)
		{
			_logger = logger;
			_marketingService = marketingService;
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
		public IActionResult HomeForManager()
		{
			var model = new HomeForManagerModel();
			model.Leads = _marketingService.GetLeads();

            return View(model);
		}

    }
}