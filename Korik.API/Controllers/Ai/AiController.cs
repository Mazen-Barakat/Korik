using Korik.Application;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace Korik.API.Controllers.Ai
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AiController : ControllerBase
    {
        #region Dependency Injection

        private readonly IMediator _mediator;

        public AiController(IMediator mediator)
        {
            _mediator = mediator;
        }

        #endregion Dependency Injection

        #region Queries

        [HttpPost("ask")]
        [SwaggerOperation(
            Summary = "Ask AI assistant a question",
            Description = "Sends a message to the AI assistant and retrieves a response based on user context."
        )]
        public async Task<IActionResult> Ask([FromBody] string message)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var request = new AskAiRequest
            {
                UserId = userId,
                UserMessage = message
            };

            var result = await _mediator.Send(request);

            return ApiResponse.FromResult(this, result);
        }

        #endregion Queries
    }
}
