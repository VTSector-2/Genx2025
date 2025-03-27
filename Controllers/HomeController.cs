using Hackathon.Interfaces;
using Hackathon.Models;
using Hackathon.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Hackathon.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly IDataService _dataService;
		private readonly IGptService _gptService;

		public HomeController(ILogger<HomeController> logger, IDataService dataService, IGptService gptService)
		{
			_logger = logger;
			_dataService = dataService;
			_gptService = gptService;
		}

		public IActionResult Index()
		{
			//default number of records are 50
			var data = _dataService.GetSiteData(numberOfRecords: 50);
			return View(data);
		}

		public IActionResult Privacy()
		{
			//default number of records are 50
			var data = _dataService.GetRiskData(0);
			var selectedRisk = data.FirstOrDefault();

			// Fetch data based on the id
			var viewModel = _dataService.GetDashboardData(data, siteId: selectedRisk.Site_Pk, siteName: selectedRisk.SiteName);

            return View(viewModel);
        }

		public DashboardViewModel? GetDashboardData(int id, string siteName)
		{
			//default number of records are 50
			var data = _dataService.GetRiskData(0);

			// Fetch data based on the id
			var viewModel = _dataService.GetDashboardData(data, siteId: id, siteName: siteName);
			return viewModel;
		}

        public DashboardViewModel? saveManualScore(int id, string siteName)
        {
            //default number of records are 50
            var data = _dataService.GetRiskData(0);

            // Fetch data based on the id
            var viewModel = _dataService.GetDashboardData(data, siteId: id, siteName: siteName);
            return viewModel;
        }

        public IActionResult DataLoader()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
