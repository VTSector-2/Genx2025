using Hackathon.Models;

namespace Hackathon.Interfaces
{
    public interface IDataService
    {
        List<Risk> GetRiskData(int numberOfRecords);
        List<Site> GetSiteData(int numberOfRecords);
        DashboardViewModel? GetDashboardData(List<Risk> data, int siteId, string siteName);

        List<SITE_SCORE_PARAM> GetSiteScoreData(int siteId, int numberOfRecords);
    }
}
