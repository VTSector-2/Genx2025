using Hackathon.Models;

namespace Hackathon.Interfaces
{
    public interface IGptService
    {
        Task<string> GetOpenAIResponse(string prompt);
        Task<string> GetOpenAIResponse(string prompt, List<string> history);

        Task<RiskAnalysisViewModel> GetRiskDataAnalysis(List<Risk> riskData);
        Task<SafetyAnalysisViewModel> GetSiteDataAnalysis(int sitePK);
    }
}
