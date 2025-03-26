using System.ComponentModel.DataAnnotations;

namespace Hackathon.Models
{
    public class Risk
    {
        [Key]
        public int SiteId_Pk { get; set; }
        public string? SiteName { get; set; }
        public string? RiskTitle { get; set; } 
        public string? RiskDescription { get; set; } 
        public string? RiskCategory { get; set; }
        public string? RiskSubCategory { get; set; }
        public string? Likelihood { get; set; }
        public string? Impact { get; set; }
        public string? RiskLevel { get; set; }
        public string? RiskStatus { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime LastUpdatedDateTime { get; set; }
    }
}
