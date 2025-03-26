using Hackathon.Models;

namespace Hackathon.Interfaces
{
    public interface IGptService
    {
        Task<string> GetOpenAIResponse(string prompt);

        Task<List<GptQuestionnaire>> GetRiskDataAnalysis(List<Risk> riskData);
        Task<SafetyAnalysisViewModel> GetSiteDataAnalysis(int sitePK);
    }
}
