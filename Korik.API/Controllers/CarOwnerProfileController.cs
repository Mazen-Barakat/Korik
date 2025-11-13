using Korik.Application;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Korik.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
            var result = await _mediator.Send(new CreateCarOwnerProfileRequest(model));

            return ApiResponse.FromResult(this, result);
        }

        #endregion Commands
    }
}