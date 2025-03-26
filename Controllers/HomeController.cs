using Hackathon.Interfaces;
using Hackathon.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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
            var data = _dataService.GetRiskData(numberOfRecords: 50);
            var analysis = _gptService.GetRiskDataAnalysis(data);
            var viewModel = new DashboardViewModel
            {
                Sites = data.Select(s => new Site() { Site_PK = s.SiteId_Pk, Site_Name = s.SiteName }).DistinctBy(d=>d.Site_Name).ToList(),
            };

            return View(viewModel);
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
