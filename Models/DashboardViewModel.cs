namespace Hackathon.Models
{
    public class DashboardViewModel
    {
        public List<Site> Sites { get; set; }
        public RiskViewModel RiskRegister { get; set; }
        public RiskAnalysisViewModel RsikAnalysis { get; set; }
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

}
