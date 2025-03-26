namespace Hackathon.Models
{
    public class DashboardViewModel
    {
        public List<Site> Sites { get; set; }
        public RsikViewModel RiskRegister { get; set; }
        public RsikAnalysisViewModel RsikAnalysis { get; set; }
        public SafetyAnalysisViewModel SafetyAnalysis { get; set; }
    }

    public class RiskViewModel
    {
        public int Low { get; set; }
        public int Negligible { get; set; }
        public int Serious { get; set; }
        public int Critical { get; set; }
    }

    public class RiskAnalysisViewModel
    {
        public string RiskProbabilityPrediction { get; set; }
        public string RiskAnalysis { get; set; }
        public string SummaryofRisks { get; set; }
        public string Recommendations { get; set; }
    }

    public class SafetyAnalysisViewModel
    {
        public List<string> ContributingFactors { get; set; }
        public List<string> MitigationPlan { get; set; }
        public List<string> LeadingIndicators { get; set; }
        public List<string> LaggingIndicators { get; set; }
        public string RiskCategory { get; set; }
        public string ManualCategory { get; set; }

    }



}
