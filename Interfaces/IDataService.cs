using Hackathon.Models;

namespace Hackathon.Interfaces
{
    public interface IDataService
    {
        List<Risk> GetRiskData(int numberOfRecords);
        List<Site> GetSiteData(int numberOfRecords);
        DashboardViewModel? GetDashboardData(List<Risk> data, int siteId, string siteName);

        Task<bool> saveManualScore(int siteId, string score);
    }
}
