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
            var selectedRsik = data.Where(w=>w.SiteName == "Site20").GroupBy(i => i.Impact)
                                                    .Select(ig => new
                                                    {
                                                        ImpactName = ig.Key,
                                                        Count = ig.Count()
                                                    });


            var analysis = _gptService.GetRiskDataAnalysis(data.Where(x=>x.SiteName == "Site20").ToList());
            var viewModel = new DashboardViewModel
            {
                Sites = data.Select(s => new Site() { Site_PK = s.SiteId_Pk, Site_Name = s.SiteName }).DistinctBy(d=>d.Site_Name).ToList(),
                RiskRegister = new RsikViewModel()
                {
                    Critical = selectedRsik.Where(w=>w.ImpactName == "Critical").Select(s=>s.Count).FirstOrDefault(),
                    Low = selectedRsik.Where(w=>w.ImpactName == "Low").Select(s=>s.Count).FirstOrDefault(),
                    Negligible = selectedRsik.Where(w=>w.ImpactName == "Negligible").Select(s=>s.Count).FirstOrDefault(),   
                    Serious = selectedRsik.Where(w=>w.ImpactName == "Serious").Select(s=>s.Count).FirstOrDefault()
                },
                RsikAnalysis = new RsikAnalysisViewModel()
                {
                    RiskProbabilityPrediction = analysis?.Result?.FirstOrDefault()?.RiskProbabilityPrediction["Answer"],
                    RiskAnalysis = analysis?.Result?.FirstOrDefault()?.RiskAnalysis["Answer"],
                    SummaryofRisks = analysis?.Result?.FirstOrDefault()?.SummaryofRisks["Answer"],
                    Recommendations = analysis?.Result?.FirstOrDefault()?.Recommendations["Answer"]
                }

            };

            return View(viewModel);
        }

        public ActionResult GetDashboardData(int id)
        {
            // Fetch data based on the id
            //var data = YourService.GetDataById(id);
            //return PartialView("_YourPartialView", data); // Return a partial view with the data
            return null;
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
