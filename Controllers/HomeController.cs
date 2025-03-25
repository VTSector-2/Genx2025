using Hackathon.DB;
using Hackathon.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;

namespace Hackathon.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _dbContext;

        public HomeController(ILogger<HomeController> logger,ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var data = _dbContext.Mast_Site
                .Where(site => !string.IsNullOrEmpty(site.Site_Code)
                    && !string.IsNullOrEmpty(site.Site_Address_1)
                    && !string.IsNullOrEmpty(site.Site_Status)
                    && site.Site_Latitude != default(decimal)
                    && site.Site_Longitude != default(decimal))
                .Take(50).ToList();
            return View(data);
        }

        public IActionResult Privacy()
        {
            //var data = _dbContext.Risk.
            //    Where(risk => !string.IsNullOrEmpty(risk.SiteName)
            //        && !string.IsNullOrEmpty(risk.RiskTitle)
            //        && !string.IsNullOrEmpty(risk.RiskDescription)
            //        && !string.IsNullOrEmpty(risk.RiskCategory)
            //        && !string.IsNullOrEmpty(risk.RiskSubCategory)
            //        && !string.IsNullOrEmpty(risk.Likelihood)
            //        && !string.IsNullOrEmpty(risk.Impact)
            //        && !string.IsNullOrEmpty(risk.RiskLevel)
            //        && !string.IsNullOrEmpty(risk.RiskStatus)
            //        && !string.IsNullOrEmpty(risk.CreatedDateTime)
            //        && !string.IsNullOrEmpty(risk.LastUpdatedDateTime))
            //    .Take(50).ToList(); ;

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
