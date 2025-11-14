using Korik.Application;
using Korik.Domain;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Korik.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "CAROWNER")]
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
        public async Task<IActionResult> PostCarOwnerProfile([FromBody] CreateCarOwnerProfileDTO model)
        {
            // Get user ID from JWT token
            var applictionUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            model.ApplicationUserId = applictionUserId!;

            var result = await _mediator.Send(new CreateCarOwnerProfileRequest(model));

            return ApiResponse.FromResult(this, result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCarOwnerProfile([FromBody] UpdateCarOwnerProfileDTO model)
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
        public async Task<IActionResult> GetCarOwnerProfile()
        {
            // Get user ID from JWT token
            var applictionUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _mediator.Send(new GetCarOwnerProfileByIdQuery(new GetCarOwnerProfileByIdDTO { ApplicationUserId = applictionUserId! }));

            return ApiResponse.FromResult(this, result);
        }

        [HttpGet("profile-with-cars")]
        public async Task<IActionResult> GetCarOwnerProfileWithCars()
        {
            // Get user ID from JWT token
            var applictionUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _mediator.Send(new GetCarOwnerProfileByIdWithCarQuery(new GetCarOwnerProfileByIdDTO { ApplicationUserId = applictionUserId! }));

            return ApiResponse.FromResult(this, result);
        }

        #endregion Queries
    }
}