namespace Hackathon.Models
{
    public class DashboardViewModel
    {
        public List<Site> Sites { get; set; }
        public RsikViewModel RiskRegister { get; set; }
        public RsikAnalysisViewModel RsikAnalysis { get; set; }
    }

    public class RsikViewModel
    {
        public int Low { get; set; }
        public int Negligible { get; set; }
        public int Serious { get; set; }
        public int Critical { get; set; }
        
    }

    public class RsikAnalysisViewModel
    {
        public string RiskProbabilityPrediction { get; set; }
        public string RiskAnalysis { get; set; }
        public string SummaryofRisks { get; set; }
        public string Recommendations { get; set; }

    }

}
