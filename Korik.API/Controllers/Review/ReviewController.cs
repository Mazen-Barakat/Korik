using Korik.Application;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Korik.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ReviewController : ControllerBase
    {
        #region Dependency Injection
        private readonly IMediator _mediator;

        public ReviewController(IMediator mediator)
        {
            _mediator = mediator;
        }

        #endregion

        [HttpPost]
        [SwaggerOperation(Summary = "Create a new review")]
        public async Task<IActionResult> PostReview([FromBody] CreateReviewDTO model)
        {
            var result = await _mediator.Send(new CreateReviewRequest(model));
            return ApiResponse.FromResult(this, result);
        }
    }
}
