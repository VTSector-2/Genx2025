using Azure;
using Azure.AI.OpenAI;
using Hackathon.Interfaces;
using Hackathon.Models;
using Microsoft.Extensions.Options;
using OpenAI.Chat;

namespace Hackathon.Services
{
    public class GptService : IGptService
    {
        private readonly AzureOpenAIClient _openAIClient;
        private readonly string _deploymentName;
        private readonly ApiSettings _apiSettings;

        public GptService(IOptions<ApiSettings> apiSettings)
        {
            _apiSettings = apiSettings.Value;
            _openAIClient = new AzureOpenAIClient(new Uri(_apiSettings.BaseUrl), new AzureKeyCredential(_apiSettings.ApiKey));
            _deploymentName = _apiSettings.DeploymentName;
        }

        public async Task<string> GetOpenAIResponse(string prompt)
        {
            var chatClient = _openAIClient.GetChatClient(_deploymentName);

            var messages = new List<ChatMessage>
            {
                new SystemChatMessage("You are a helpful assistant."),
                new UserChatMessage(prompt)
            };

            ChatCompletion completion;

            try
            {
                completion = await chatClient.CompleteChatAsync(messages);
            }
            catch (Exception ex)
            {
                return $"⚠️ Error generating response: {ex.Message}";
            }

            return completion.Content[0].Text.ToString();
        }

        public List<string> CreateRiskPromptAsync(List<Risk> riskData)
        {
            var prompts = new List<string>();
            var siteNames = riskData.Select(r => r.SiteName).Distinct();

            foreach (var siteName in siteNames)
            {
                var currentSiteRiskData = riskData.Where(r => r.SiteName == siteName).ToList();
                var prompt = $"{{\"siteName\": \"{siteName}\",\"risks\": [";

                foreach (var risk in currentSiteRiskData)
                {
                    prompt += $"{{\"riskTitle\": \"{risk.RiskTitle}\",\"riskDescription\": \"{risk.RiskDescription}\",\"likelihood\": \"{risk.Likelihood}\",\"impact\": \"{risk.Impact}\",\"riskLevel\": \"{risk.RiskLevel}\"}},";
                }

                if (currentSiteRiskData.Count > 0)
                {
                    prompt = prompt.TrimEnd(',');
                }

                prompt += "]}}";
                prompts.Add(prompt);
            }

            return prompts;
        }
    }
}
