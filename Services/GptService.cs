using Azure;
using Azure.AI.OpenAI;
using Hackathon.Interfaces;
using Hackathon.Models;
using Microsoft.Extensions.Options;
using OpenAI.Chat;
using Newtonsoft.Json.Linq;

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
                new SystemChatMessage("You are a helpful assistant who has access to my application database. I need help with this."),
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

        public async Task<List<GptQuestionnaire>> GetRiskDataAnalysis(List<Risk> riskData)
        {
            var jsonData = CreateRiskPromptAsync(riskData);
            List<GptQuestionnaire> questionnaires = new List<GptQuestionnaire>();
            foreach (var jsonDataItem in jsonData)
            {
                JObject? jsonObject = JObject.Parse(jsonDataItem);
                string siteName = jsonObject?["siteName"]?.ToString() ?? string.Empty;
                string risks = jsonObject?["risks"]?.ToString() ?? string.Empty;

                var questionnaire = new GptQuestionnaire
                {
                    RiskProbabilityPrediction = new Dictionary<string, string>
                    {
                        { "Question", "Predict the probability of the following risks occurring in the future for" + siteName + ": " + risks + ". Provide only numerical probabilities or categorical likelihoods (e.g., low, medium, high). Analyze all and show only Top 10 risks which have most severe impact." },
                        { "Answer", await GetOpenAIResponse("Predict the probability of the following risks occurring in the future for" + siteName + ": " + risks + ". Provide only numerical probabilities or categorical likelihoods (e.g., low, medium, high). Analyze all and show only Top 10 risks which have most severe impact.") }
                    },
                    RiskAnalysis = new Dictionary<string, string>
                    {
                        { "Question", "Analyze the risks for " + siteName + " based on the following data: " + risks + ". Identify key risk factors and provide a list of recommendations for risk mitigation. Do not include any introductory or concluding remarks." },
                        { "Answer", await GetOpenAIResponse("Analyze the risks for " + siteName + " based on the following data: " + risks + ". Identify key risk factors and provide a list of recommendations for risk mitigation. Do not include any introductory or concluding remarks.") }
                    },
                    SummaryofRisks = new Dictionary<string, string>
                    {
                        { "Question", "Summarize the risks for " + siteName + " considering the likelihood and impact of each risk: " + risks + ". Provide a structured summary without additional commentary." },
                        { "Answer", await GetOpenAIResponse("Summarize the risks for " + siteName + " considering the likelihood and impact of each risk: " + risks + ". Provide a structured summary without additional commentary.") }
                    },
                    Recommendations = new Dictionary<string, string>
                    {
                        { "Question", "Provide strategies for risk management and mitigation based on the identified risks for " + siteName + ": " + risks + ". List recommendations clearly and concisely without conversational phrases." },
                        { "Answer", await GetOpenAIResponse("Provide strategies for risk management and mitigation based on the identified risks for " + siteName + ": " + risks + ". List recommendations clearly and concisely without conversational phrases.") }
                    }
                };

                questionnaires.Add(questionnaire);
            }

            return questionnaires;
        }

        #region private methods 
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

                prompt += "]}";
                prompts.Add(prompt);
            }

            return prompts;
        }
        #endregion
    }
}
