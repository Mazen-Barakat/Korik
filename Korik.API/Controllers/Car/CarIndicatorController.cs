using Korik.Application;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Korik.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarIndicatorController : ControllerBase
    {
        #region Dependency Injection
        private readonly IMediator _mediator;
        public CarIndicatorController(IMediator mediator)
        {
            _mediator = mediator;
        }

        #endregion

        #region Commands
        [HttpPost]
        [SwaggerOperation(Summary = "Create a new car Indicator")]
        public async Task<IActionResult> PostCarIndicator([FromBody] CreateCarIndicatorDTO model)
        {
            var result = await _mediator.Send(new CreateCarIndicatorRequest(model));
            return ApiResponse.FromResult(this, result);
        }

        [HttpDelete("{id:int}")]
        [SwaggerOperation(Summary = "Delete a car indicator by Id")]
        public async Task<IActionResult> DeleteCarIndicator([FromRoute] int id)
        {
            var result = await _mediator.Send(new DeleteCarIndicatorRequest
                (
                new DeleteCarIndicatorDTO() { Id = id}
                ));
            return ApiResponse.FromResult(this, result);
        }

        [HttpPut]
        [SwaggerOperation(Summary = "Update a car indicator by Id")]
        public async Task<IActionResult> PutCarIndicator([FromBody] UpdateCarIndicatorDTO model)
        {
            var result = await _mediator.Send(new UpdateCarIndicatorRequest(model));
            return ApiResponse.FromResult(this, result);
        }
        #endregion

        #region Queries
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get a car indicator by Id")]
        public async Task<IActionResult> GetCarIndicatorById(int id)
        {
            var result = await _mediator.Send(new GetByIdCarIndicatorRequest(new GetByIdCarIndicatorDTO { Id = id }));
            return ApiResponse.FromResult(this, result);
        }

        [HttpGet("car/{carId}")]
        [SwaggerOperation(Summary = "Get all car indicators by Car Id")]
        public async Task<IActionResult> GetAllCarIndicatorsByCarId(int carId)
        {
            var result = await _mediator.Send(new GetAllCarIndicatorsByCarIdRequest(new GetAllIndicatorsByCarIdDTO { CarId = carId }));
            return ApiResponse.FromResult(this, result);
        }
        #endregion
    }
}
