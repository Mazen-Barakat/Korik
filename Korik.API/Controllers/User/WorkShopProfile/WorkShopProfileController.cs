using Korik.Application;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Korik.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        public async Task<IActionResult> UpdateWorkShopProfile([FromForm] UpdateWorkShopProfileDTO model)
        {
            // Get user ID from JWT token
            var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            model.ApplicationUserId = applicationUserId;

            var result = await _mediator.Send(new UpdateWorkShopProfileRequest(model));

            return ApiResponse.FromResult(this, result);
        }

        [HttpPut("Update-WorkShop-Profile-Status")]
        public async Task<IActionResult> UpdateWorkShopProfileStatus([FromBody] UpdateWorkShopProfileStatusDTO model)
        {
            var result = await _mediator.Send(new UpdateWorkShopProfileStatusRequest(model));

            return ApiResponse.FromResult(this, result);
        }

        #endregion Commands

        #region Queries

        [HttpGet("Get-All-WorkShop-Profiles")]
        public async Task<IActionResult> GetWorkShopProfiles([FromQuery] PagedRequestDTO model)
        {
            var result = await _mediator.Send(new GetAllWorkShopProfileRequest(model));

            return ApiResponse.FromResult(this, result);
        }

        [HttpGet("Get-All-WorkShop-Profiles-By-service")]
        public async Task<IActionResult> GetWorkShopProfilesByServices([FromQuery] GetAllWorkShopProfilesByServiceDTO model)
        {
            var result = await _mediator.Send(new GetAllWorkShopProfilesByServiceRequest(model));

            return ApiResponse.FromResult(this, result);
        }

        [HttpGet("Get-My-WorkShop-Profile")]
        public async Task<IActionResult> GetMyWorkShopProfile()
        {
            // Get user ID from JWT token
            var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _mediator.Send(new GetMyWorkShopProfileByIdRequest(new GetMyWorkShopProfileByIdDTO { ApplicationUserId = applicationUserId }));

            return ApiResponse.FromResult(this, result);
        }

        [HttpGet("Get-WorkShop-ById-Profile")]
        public async Task<IActionResult> GetWorkShopProfileById([FromQuery] int id)
        {
            var result = await _mediator.Send(new GetWorkShopProfileByIdRequest(new GetWorkShopProfileByIdDTO { Id = id }));

            return ApiResponse.FromResult(this, result);
        }

        #endregion Queries
    }
}