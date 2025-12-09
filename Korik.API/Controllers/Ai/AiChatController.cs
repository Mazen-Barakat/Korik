using Korik.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Korik.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AiChatController : ControllerBase
    {
        private readonly IAiAssistantService _aiAssistantService;

        public AiChatController(IAiAssistantService aiAssistantService)
        {
            _aiAssistantService = aiAssistantService;
        }

        /// <summary>
        /// Send a message to the AI assistant and receive a response.
        /// Conversation context is maintained using sessionId.
        /// </summary>
        /// <param name="request">The chat message and optional session ID</param>
        /// <returns>AI response with conversation context</returns>
        [HttpPost]
        public async Task<IActionResult> Chat([FromBody] CreateAiChatDTO request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated");
            }

            if (string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest("Message cannot be empty");
            }

            var result = await _aiAssistantService.ProcessQueryAsync(
                userId,
                request.Message,
                request.SessionId);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Clear the conversation history for the current session.
        /// Use this to start a fresh conversation.
        /// </summary>
        /// <param name="sessionId">Optional session ID to clear (clears default session if not provided)</param>
        [HttpDelete("clear")]
        public IActionResult ClearConversation([FromQuery] string? sessionId = null)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated");
            }

            _aiAssistantService.ClearConversation(userId, sessionId);

            return Ok(new { message = "Conversation cleared successfully" });
        }

        /// <summary>
        /// Start a new conversation session and return the session ID.
        /// </summary>
        [HttpPost("new-session")]
        public IActionResult CreateNewSession()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated");
            }

            var sessionId = Guid.NewGuid().ToString("N");

            return Ok(new
            {
                sessionId,
                message = "New conversation session created. Use this sessionId in subsequent requests to maintain context."
            });
        }
    }
}
