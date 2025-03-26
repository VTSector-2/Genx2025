namespace Hackathon.Models
{
    public class GptQuestionnaire
    {
        public Dictionary<string, string> RiskProbabilityPrediction { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> RiskAnalysis { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> SummaryofRisks { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> Recommendations { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> AdditionalQuestions { get; set; } = new Dictionary<string, string>();
    }
}
