using Hackathon.DataContext;
using Hackathon.Interfaces;
using Hackathon.Models;

namespace Hackathon.Services
{
	public class DataService : IDataService
	{
		private readonly ApplicationDbContext _dbContext;
		private readonly IGptService _gptService;

		public DataService(ApplicationDbContext dbContext,
			IGptService gptService)
		{
			_dbContext = dbContext;
			_gptService = gptService;
		}

		public List<Risk> GetRiskData(int numberOfRecords)
		{
			if(numberOfRecords == 0)
			{
               return _dbContext.Risk.ToList();
            }
            // Get data from the database
            return _dbContext.Risk.Take(numberOfRecords).ToList();
		}

		public List<Site> GetSiteData(int numberOfRecords)
		{
			// Get data from the database
			var data = _dbContext.Mast_Site
			   .Where(site => !string.IsNullOrEmpty(site.Site_Code)
				   && !string.IsNullOrEmpty(site.Site_Address_1)
				   && !string.IsNullOrEmpty(site.Site_Status)
				   && site.Site_Latitude != default(decimal)
				   && site.Site_Longitude != default(decimal))
			   .Take(numberOfRecords).ToList();

			return data;
		}

		public DashboardViewModel? GetDashboardData(List<Risk> data, int siteId, string siteName)
		{
			var selectedRsik = data.Where(w => w.SiteName == siteName).GroupBy(i => i.Impact)
				.Select(ig => new
				{
					ImpactName = ig.Key,
					Count = ig.Count()
				});

			var analysis = _gptService.GetRiskDataAnalysis(data.Where(x => x.SiteName == siteName).ToList());
            var siteScore = _gptService.GetSiteDataAnalysis(siteId);
            var viewModel = new DashboardViewModel
			{
				Sites = data.Select(s => new Site() { Site_PK = s.Site_Pk, Site_Name = s.SiteName }).DistinctBy(d => d.Site_Name).ToList(),
				RiskRegister = new RiskViewModel()
				{
					Critical = selectedRsik.Where(w => w.ImpactName == "Critical").Select(s => s.Count).FirstOrDefault(),
					Low = selectedRsik.Where(w => w.ImpactName == "Low").Select(s => s.Count).FirstOrDefault(),
					Negligible = selectedRsik.Where(w => w.ImpactName == "Negligible").Select(s => s.Count).FirstOrDefault(),
					Serious = selectedRsik.Where(w => w.ImpactName == "Serious").Select(s => s.Count).FirstOrDefault()
				},
				RsikAnalysis = analysis.Result,
				SafetyAnalysis = siteScore.Result
            };

			return viewModel;
		}
    }
}
