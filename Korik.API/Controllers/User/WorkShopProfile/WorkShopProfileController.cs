using Korik.Application;
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
    [Authorize(Roles = "CAROWNER,WORKSHOP,ADMIN")]

    public class WorkShopProfileController : ControllerBase
    {
        #region Dependency Injection

        private readonly IMediator _mediator;

        public WorkShopProfileController(IMediator mediator)
        {
            _mediator = mediator;
        }

        #endregion Dependency Injection

        #region Commands

        [HttpPut("Update-WorkShop-Profile")]
        [Authorize(Roles = "WORKSHOP,ADMIN")]
        [SwaggerOperation(Summary = "Update a workshop profile",
                          Description = "This endpoint updates an existing workshop profile with new details such as name, address, contact information, etc.")]
        public async Task<IActionResult> UpdateWorkShopProfile([FromForm] UpdateWorkShopProfileDTO model)
        {
            // Get user ID from JWT token
            var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            model.ApplicationUserId = applicationUserId;

            var result = await _mediator.Send(new UpdateWorkShopProfileRequest(model));

            return ApiResponse.FromResult(this, result);
        }

        [HttpPut("Update-WorkShop-Profile-Status")]
        [Authorize(Roles = "ADMIN")]
        [SwaggerOperation(Summary = "Update the status of a workshop profile",
                          Description = "This endpoint updates the status (e.g., verified/unverified) of a specific workshop profile.")]
        public async Task<IActionResult> UpdateWorkShopProfileStatus([FromBody] UpdateWorkShopProfileStatusDTO model)
        {
            var result = await _mediator.Send(new UpdateWorkShopProfileStatusRequest(model));

            return ApiResponse.FromResult(this, result);
        }

        #endregion Commands

        #region Queries

        [HttpGet("Get-All-WorkShop-Profiles")]
        [SwaggerOperation(Summary = "Get all verified workshop profiles",
                          Description = "This endpoint retrieves a paginated list of all verified workshop profiles.")]
        public async Task<IActionResult> GetWorkShopProfiles([FromQuery] PagedRequestDTO model)
        {
            var result = await _mediator.Send(new GetAllWorkShopProfileRequest(model));

            return ApiResponse.FromResult(this, result);
        }

        [HttpGet("Get-All-WorkShop-Profiles-By-service-Id")]
        [SwaggerOperation(Summary = "Get workshop profiles filtered by service",
                          Description = "This endpoint retrieves workshop profiles that offer specific services.")]
        public async Task<IActionResult> GetWorkShopProfilesByServices([FromQuery] GetAllWorkShopProfilesByServiceDTO model)
        {
            var result = await _mediator.Send(new GetAllWorkShopProfilesByServiceRequest(model));

            return ApiResponse.FromResult(this, result);
        }

        [HttpGet("Get-My-WorkShop-Profile")]
        [Authorize(Roles = "WORKSHOP")]
        [SwaggerOperation(Summary = "Get the current user's workshop profile",
                          Description = "This endpoint retrieves the current authenticated user's workshop profile based on their user ID.")]
        public async Task<IActionResult> GetMyWorkShopProfile()
        {
            // Get user ID from JWT token
            var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _mediator.Send(new GetMyWorkShopProfileByIdRequest(new GetMyWorkShopProfileByIdDTO { ApplicationUserId = applicationUserId }));

            return ApiResponse.FromResult(this, result);
        }

        [HttpGet("Get-WorkShop-ById-Profile")]
        [SwaggerOperation(Summary = "Get a workshop profile by ID",
                          Description = "This endpoint retrieves a specific workshop profile based on the provided workshop ID.")]
        public async Task<IActionResult> GetWorkShopProfileById([FromQuery] int id)
        {
            var result = await _mediator.Send(new GetWorkShopProfileByIdRequest(new GetWorkShopProfileByIdDTO { Id = id }));

            return ApiResponse.FromResult(this, result);
        }

        [HttpGet("Get-All-Unverified-WorkShop-Profile")]
        [SwaggerOperation(Summary = "Get all unverified workshop profiles",
                          Description = "This endpoint retrieves a paginated list of all unverified workshop profiles.")]
        public async Task<IActionResult> GetAllUnverifiedWorkShopProfile([FromQuery] PagedRequestDTO model)
        {
            var result = await _mediator.Send(new GetAllUnverifiedWorkShopProfileRequest(model));

            return ApiResponse.FromResult(this, result);
        }

        #endregion Queries
    }
}