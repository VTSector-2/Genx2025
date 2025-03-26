using Hackathon.Models;

namespace Hackathon.Interfaces
{
    public interface IGptService
    {
        Task<string> GetOpenAIResponse(string prompt);

        List<string> CreateRiskPromptAsync(List<Risk> data);
    }
}
