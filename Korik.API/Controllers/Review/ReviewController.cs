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

        #endregion Dependency Injection

        #region Commands

        [HttpPost]
        [SwaggerOperation(Summary = "Create a new review")]
        public async Task<IActionResult> PostReview([FromBody] CreateReviewDTO model)
        {
            var result = await _mediator.Send(new CreateReviewRequest(model));
            return ApiResponse.FromResult(this, result);
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete an existing review")]
        public async Task<IActionResult> DeleteReview([FromRoute] int id)
        {
            var result = await _mediator.Send(new DeleteReviewRequest(new DeleteReviewDTO { Id = id }));
            return ApiResponse.FromResult(this, result);
        }

        [HttpPut]
        [SwaggerOperation(Summary = "Update an existing review")]
        public async Task<IActionResult> PutReview([FromBody] UpdateReviewDTO model)
        {
            var result = await _mediator.Send(new UpdateReviewRequest(model));
            return ApiResponse.FromResult(this, result);
        }

        #endregion Commands



        #region Queries

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get a review by ID")]
        public async Task<IActionResult> GetReviewById([FromRoute] int id)
        {
            var result = await _mediator.Send(new GetByIdReviewRequest(new GetReviewByIdDTO { Id = id }));
            return ApiResponse.FromResult(this, result);
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get all reviews")]
        public async Task<IActionResult> GetAllReviews()
        {
            var result = await _mediator.Send(new GetAllReviewsRequest());
            return ApiResponse.FromResult(this, result);
        }

        [HttpGet("average-rating/{workShopProfileId}")]
        [SwaggerOperation(Summary = "Get average ratings by WorkShopProfileId")]
        public async Task<IActionResult> GetAverageRatingsByWorkShopProfileId([FromRoute] int workShopProfileId)
        {
            var result = await _mediator.Send(new GetAvgRatingsByWorkShopProfileIdRequest(new GetAvgRatingsByWorkShopProfileIdDTO { WorkShopProfileId = workShopProfileId }));
            return ApiResponse.FromResult(this, result);
        }

        [HttpGet("all-Review/{workShopProfileId}")]
        [SwaggerOperation(Summary = "Get all Review by WorkShopProfileId")]
        public async Task<IActionResult> GetAllRatingsByWorkShopProfileId([FromRoute] int workShopProfileId)
        {
            var result = await _mediator.Send(new GetAllRatingsByWorkShopProfileIdRequest(new GetAllRatingsByWorkShopProfileIdDTO { WorkShopProfileId = workShopProfileId }));
            return ApiResponse.FromResult(this, result);
        }

        #endregion Queries
    }
}