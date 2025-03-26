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

            string siteName = "Site20";
            // Process the user message and generate a response
            string reply = "";

            var dbResponse = await GetResponseFromDatabase(userMessage.Message, siteName, userMessage.History);
            if (!string.IsNullOrEmpty(dbResponse))
            {
                reply = dbResponse; // Use the database response if found
            }
            else
            {
                reply = await _aiService.GetOpenAIResponse(userMessage.Message, userMessage.History);
            }
            return Ok(new { reply });
        }

        private async Task<string> GetResponseFromDatabase(string userMessage, string siteName, List<string> userHistory)
        {
            // Fetch risks associated with the specified site
            var risks = await _context.Risks
                .Where(r => r.SiteName == siteName) // Filter by SiteName
                .ToListAsync();

            if (risks.Any())
            {
                // Create a structured prompt for the AI
                var riskData = new StringBuilder();
                riskData.AppendLine("{\"risks\": [");

                foreach (var risk in risks)
                {
                    riskData.AppendLine($"{{\"siteId\": {risk.SiteId_Pk}, \"title\": \"{risk.RiskTitle}\", \"description\": \"{risk.RiskDescription}\", \"category\": \"{risk.RiskCategory}\",  \"likelihood\": \"{risk.Likelihood}\", \"impact\": \"{risk.Impact}\",\"status\": \"{risk.RiskStatus}\"}},");
                }

                // Remove the last comma and close the JSON array
                if (risks.Count > 0)
                {
                    riskData.Length--; // Remove the last comma
                }
                riskData.AppendLine("]}");

                // Send risk data to AI for processing
                string aiResponse = await _aiService.GetOpenAIResponse($"Consider the data for the site with name {siteName}: {riskData}.This is everything we have in data base for the site. All records here are associated with the given site only.\n Answer the following User Query if it relates to this data or provide a generic response or in context to the previous reponses by you: {userMessage}", userHistory);
                return aiResponse;
            }
            else
            {
                return $"No risks found associated with site name {siteName}.";
            }
        }

    }

    public class UserMessage
    {
        public string Message { get; set; }
        public List<string> History { get; set; }
    }

    public class ApiSettings
    {
        public string BaseUrl { get; set; }
        public string ApiKey { get; set; }
        public string DeploymentName { get; set; }
    }
}
