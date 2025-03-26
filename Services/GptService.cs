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

        /// <summary>
        /// Overloaded function for contexual responses.
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="history"></param>
        /// <returns></returns>
        public async Task<string> GetOpenAIResponse(string prompt, List<string> history)
        {
            var chatClient = _openAIClient.GetChatClient(_deploymentName);

            var allMessages = new List<ChatMessage>();
            foreach (var message in history)
            {
                allMessages.Add(new UserChatMessage(message)); 
            }

            allMessages.Add(new UserChatMessage(prompt));

            ChatCompletion completion;

            try
            {
                completion = await chatClient.CompleteChatAsync(allMessages);
            }
            catch (Exception ex)
            {
                return $"⚠️ Error generating response: {ex.Message}";
            }

            return completion.Content[0].Text.ToString();
        }

        public async Task<RiskAnalysisViewModel> GetRiskDataAnalysis(List<Risk> riskData)
        {
            var json = JsonConvert.SerializeObject(riskData);
            var prompt = $"please provide the Risk Probability Prediction,Risk Analysis,Summary of Risks and Recommendationsalong a small paraghra for each point with no sub point for the given site data, do not add any special charactors and line change" + json;

            var data = await GetOpenAIResponse(prompt);
            var respons = new RiskAnalysisViewModel
            {
                RiskProbabilityPrediction = GetStringBetween(data, "Risk Probability Prediction", "Risk Analysis").Replace(':',' ').Replace("###"," "),
                RiskAnalysis = GetStringBetween(data, "Risk Analysis", "Summary of Risks").Replace(':', ' ').Replace("###", " "),
                SummaryofRisks = GetStringBetween(data, "Summary of Risks", "Recommendations").Replace(':', ' ').Replace("###", " "),
                Recommendations = GetStringBetween(data, "Recommendations", null).Replace(':', ' ').Replace("###", " "),
            };

            return respons;
        }

        public async Task<SafetyAnalysisViewModel> GetSiteDataAnalysis(int sitePK)
        {
            var siteParamData = _dbContext.SITE_SCORE_PARAM
                                    .Where(site => site.SITE_PK == sitePK).FirstOrDefault();
            var json = JsonConvert.SerializeObject(siteParamData);
            var prompt = $"please provide some Contributing Factors, Mitigation Plan, Leading Indicators and Lagging Indicators along with only four and sort bulletin points without sub points for the given site data, do not add any special charactors and line change" + json;

            var data = await GetOpenAIResponse(prompt);
            var respons = new SafetyAnalysisViewModel
            {
                ContributingFactors = GetStringBetween(data, "Contributing Factors", "Mitigation Plan").Replace("\n", " ").Replace("### **", " ").Replace("*", " ").Split('-').ToList(),
                MitigationPlan = GetStringBetween(data, "Mitigation Plan", "Leading Indicators").Replace("\n", " ").Replace("### **", " ").Replace("*", " ").Split('-').ToList(),
                LeadingIndicators = GetStringBetween(data, "Leading Indicators", "Lagging Indicators").Replace("\n", " ").Replace("### **", " ").Replace("*", " ").Split('-').ToList(),
                LaggingIndicators = GetStringBetween(data, "Lagging Indicators", "---").Replace("\n", " ").Replace("### **", " ").Replace("*", " ").Split('-').ToList(),
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
            int endIndex = end == null ? source.Length - 1 : source.IndexOf(end, startIndex);

            if (startIndex < start.Length || endIndex == -1)
            {
                return string.Empty; // Return empty string if start or end not found
            }

            return source.Substring(startIndex, endIndex - startIndex);
        }
        #endregion
    }
}
