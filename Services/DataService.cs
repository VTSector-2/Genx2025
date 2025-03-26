using Hackathon.DB;
using Hackathon.Interfaces;
using Hackathon.Models;

namespace Hackathon.Services
{
    public class DataService: IDataService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IGptService _gptService;

        public DataService(ApplicationDbContext dbContext,
            IGptService gptService)
        {
            _dbContext = dbContext;
            _gptService = gptService;
        }

        public List<Risk> GetRiskData (int numberOfRecords)
        {
            // Get data from the database
            var data = _dbContext.Risk.Take(numberOfRecords).ToList();
            return data;
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
    }
}
