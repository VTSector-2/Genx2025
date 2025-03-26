using Hackathon.DataContext;
using Hackathon.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Hackathon.Interfaces;

namespace Hackathon.Controllers
{
    [Route("ChatBot")]
    [ApiController]
    public class ChatBotController : ControllerBase // Ensure you inherit from ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IGptService _aiService; // Use the interface

        public ChatBotController(ApplicationDbContext context, IGptService aiService) // Use the interface here
        {
            _context = context;
            _aiService = aiService;
        }

        [HttpPost("SendMessage")]
        public async Task<IActionResult> SendMessage([FromBody] UserMessage userMessage)
        {
            // Validate the incoming message
            if (userMessage == null || string.IsNullOrWhiteSpace(userMessage.Message))
            {
                return BadRequest(new { reply = "Please send a valid message." });
            }

            // Process the user message and generate a response
            string reply = "";

            var dbResponse = await GetResponseFromDatabase(userMessage.Message);
            if (!string.IsNullOrEmpty(dbResponse))
            {
                reply = dbResponse; // Use the database response if found
            }
            else
            {
                reply = await _aiService.GetOpenAIResponse(userMessage.Message); // Use the AI service
            }
            return Ok(new { reply });
        }

        private async Task<string> GetResponseFromDatabase(string userMessage)
        {
            // Check for risk-related keywords in the user's message
            if (userMessage.Contains("risk", StringComparison.OrdinalIgnoreCase)
                || userMessage.Contains("hazard", StringComparison.OrdinalIgnoreCase)
                || userMessage.Contains("threat", StringComparison.OrdinalIgnoreCase))
            {
                // Query the database for risks associated with the site
                var risks = await _context.Risk
                    .Where(r => r.SiteName != null && r.SiteName.Contains(userMessage, StringComparison.OrdinalIgnoreCase))
                    .ToListAsync();

                if (risks.Any())
                {
                    // Create a string to summarize the risks
                    var riskSummary = new StringBuilder();
                    riskSummary.AppendLine($"Found {risks.Count} risk(s) associated with the site:");

                    foreach (var risk in risks)
                    {
                        riskSummary.AppendLine($"- **Title**: {risk.RiskTitle}, **Description**: {risk.RiskDescription}, **Category**: {risk.RiskCategory}, **Status**: {risk.RiskStatus}");
                    }

                    return riskSummary.ToString();
                }
                else
                {
                    return "No risks found associated with the site.";
                }
            }

            // Additional checks can be added here

            return null; // Return null if no relevant data was found
        }
    }

    public class UserMessage
    {
        public string Message { get; set; }
    }

    public class ApiSettings
    {
        public string BaseUrl { get; set; }
        public string ApiKey { get; set; }
        public string DeploymentName { get; set; }
    }
}
