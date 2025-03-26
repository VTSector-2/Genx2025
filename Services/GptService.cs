using Azure;
using Azure.AI.OpenAI;
using Hackathon.Interfaces;
using Hackathon.Models;
using Microsoft.Extensions.Options;
using OpenAI.Chat;
using Newtonsoft.Json.Linq;
using Hackathon.DataContext;
using Newtonsoft.Json;

namespace Hackathon.Services
{
    public class GptService : IGptService
    {
        private readonly AzureOpenAIClient _openAIClient;
        private readonly string _deploymentName;
        private readonly ApiSettings _apiSettings;
        private readonly ApplicationDbContext _dbContext;

        public GptService(IOptions<ApiSettings> apiSettings, ApplicationDbContext dbContext)
        {
            _apiSettings = apiSettings.Value;
            _openAIClient = new AzureOpenAIClient(new Uri(_apiSettings.BaseUrl), new AzureKeyCredential(_apiSettings.ApiKey));
            _deploymentName = _apiSettings.DeploymentName;
            _dbContext = dbContext;
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
                        { "Question", "Predict the probability of the following risks occurring in the future for " + siteName + ": " + risks },
                        { "Answer", await GetOpenAIResponse("Predict the probability of the following risks occurring in the future for " + siteName + ": " + risks) }
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

        public async Task<SafetyAnalysisViewModel> GetSiteDataAnalysis(int sitePK)
        {
            var siteParamData = _dbContext.SITE_SCORE_PARAM
                                    .Where(site => site.SITE_PK == sitePK).FirstOrDefault();
            var json = JsonConvert.SerializeObject(siteParamData);
            var prompt = $"please provide some Contributing Factors, Mitigation Plan, Leading Indicators and Lagging Indicators along with only four and sort bulletin points without sub points for the given site data." + json;

            var data = await GetOpenAIResponse(prompt);
            var respons = new SafetyAnalysisViewModel
            {
                ContributingFactors = GetStringBetween(data, "### **Contributing Factors**", "### **Mitigation Plan**").Replace("\n", " ").Split('-').ToList(),
                MitigationPlan = GetStringBetween(data, "### **Mitigation Plan**", "### **Leading Indicators**").Replace("\n", " ").Split('-').ToList(),
                LeadingIndicators = GetStringBetween(data, "### **Leading Indicators**", "### **Lagging Indicators**").Replace("\n", " ").Split('-').ToList(),
                LaggingIndicators = GetStringBetween(data, "### **Lagging Indicators**", "---").Replace("\n", " ").Split('-').ToList(),
                RiskCategory = siteParamData.Risk_Category,
                ManualCategory = siteParamData.Manual_category,
            };  

            return respons;
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

        public static string GetStringBetween(string source, string start, string end)
        {
            int startIndex = source.IndexOf(start) + start.Length;
            int endIndex = source.IndexOf(end, startIndex);

            if (startIndex < start.Length || endIndex == -1)
            {
                return string.Empty; // Return empty string if start or end not found
            }

            return source.Substring(startIndex, endIndex - startIndex);
        }
        #endregion
    }
}
