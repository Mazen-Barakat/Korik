using Korik.Application;
using Korik.Domain;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Korik.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "CAROWNER,WORKSHOP")]
    public class CarOwnerProfileController : ControllerBase
    {
        #region Dependency Injection

        private readonly IMediator _mediator;

        public CarOwnerProfileController(IMediator mediator)
        {
            _mediator = mediator;
        }

        #endregion Dependency Injection

        #region Commands

        [HttpPost]
        [SwaggerOperation(Summary = "Create car Owner Profile",
                              Description = "This endpoint allows the creation of a car owner's profile with personal details including their name, phone number, address, preferred language, and an optional profile image. The provided information is used to generate and store a new car owner profile for future interactions or services.")]
        public async Task<IActionResult> PostCarOwnerProfile([FromBody] CreateCarOwnerProfileDTO model)
        {
            // Get user ID from JWT token
            var applictionUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            model.ApplicationUserId = applictionUserId!;

            var result = await _mediator.Send(new CreateCarOwnerProfileRequest(model));

            return ApiResponse.FromResult(this, result);
        }

        [HttpPut]
        [SwaggerOperation(Summary = "Update car Owner Profile")]
        public async Task<IActionResult> UpdateCarOwnerProfile([FromForm] UpdateCarOwnerProfileDTO model)
        {
            // Get user ID from JWT token
            var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            model.ApplicationUserId = applicationUserId;

            var result = await _mediator.Send(new UpdateCarOwnerProfileRequest(model));

            return ApiResponse.FromResult(this, result);
        }

        #endregion Commands

        #region Queries

        [HttpGet("profile")]
        [SwaggerOperation(Summary = "Get car Owner Profile Data only")]
        public async Task<IActionResult> GetCarOwnerProfile()
        {
            // Get user ID from JWT token
            var applictionUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _mediator.Send(new GetCarOwnerProfileByIdQuery(new GetCarOwnerProfileByIdDTO { ApplicationUserId = applictionUserId! }));

            return ApiResponse.FromResult(this, result);
        }

        [HttpGet("profile-with-cars")]
        [SwaggerOperation(Summary = "Get car Owner Profile Data with car main data")]
        public async Task<IActionResult> GetCarOwnerProfileWithCars()
        {
            // Get user ID from JWT token
            var applictionUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _mediator.Send(new GetCarOwnerProfileByIdWithCarQuery(new GetCarOwnerProfileByIdDTO { ApplicationUserId = applictionUserId! }));

            return ApiResponse.FromResult(this, result);
        }

        [HttpGet("by-booking/{bookingId:int}")]
        [SwaggerOperation(
            Summary = "Get car owner profile by booking Id",
            Description = "Retrieves the complete car owner profile data associated with the specified booking Id, including full name, contact information, and address details."
        )]
        public async Task<IActionResult> GetCarOwnerProfileByBookingId([FromRoute] int bookingId)
        {
            var result = await _mediator.Send(
                new GetCarOwnerProfileByBookingIdRequest(
                    new GetCarOwnerProfileByBookingIdDTO() { BookingId = bookingId }
                    ));
            return ApiResponse.FromResult(this, result);
        }

        #endregion Queries
    }
}