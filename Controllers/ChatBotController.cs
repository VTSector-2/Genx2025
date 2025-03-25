using Microsoft.AspNetCore.Mvc;

namespace Hackathon.Controllers
{
    [Route("ChatBot")]
    [ApiController]
    public class ChatBotController : Controller
    {
        [HttpPost("SendMessage")]
        public IActionResult SendMessage([FromBody] UserMessage userMessage)
        {
            // Validate the incoming message
            if (userMessage == null || string.IsNullOrWhiteSpace(userMessage.Message))
            {
                return BadRequest(new { reply = "Please send a valid message." });
            }

            // Process the user message and generate a response
            string reply = GenerateReply(userMessage.Message);
            return Ok(new { reply });
        }

        private string GenerateReply(string userMessage)
        {
            // Example logic for generating a reply based on user input
            if (userMessage.Contains("incident", StringComparison.OrdinalIgnoreCase))
            {
                return "You can check the latest incidents in the incidents section.";
            }
            else if (userMessage.Equals("hi", StringComparison.OrdinalIgnoreCase))
            {
                return "Hi. How may I help you today?";
            }
            else if (userMessage.Equals("hello", StringComparison.OrdinalIgnoreCase))
            {
                return "Hello. You need any help?";
            }
            else if (userMessage.Contains("observation", StringComparison.OrdinalIgnoreCase))
            {
                return "Observations can be logged in the observations section.";
            }
            else if (userMessage.Contains("action", StringComparison.OrdinalIgnoreCase))
            {
                return "You can view actions taken in the actions section.";
            }
            else if (userMessage.Contains("investigation", StringComparison.OrdinalIgnoreCase))
            {
                return "Investigations are recorded in the investigations section.";
            }
            else if (userMessage.Contains("audit", StringComparison.OrdinalIgnoreCase))
            {
                return "Audits are available in the audits section.";
            }

            // Default response if no specific keywords are matched
            return "I'm sorry, I did not understand that. Can you please rephrase your question? or ask something related to the selected site only as I dont have access to any other data here.";
        }
    }

    public class UserMessage
    {
        public string Message { get; set; }
    }
}
