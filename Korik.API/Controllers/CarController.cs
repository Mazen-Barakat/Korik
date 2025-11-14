using Korik.Application;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Korik.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class CarController : ControllerBase
    {
        #region Dependency Injection

        private readonly IMediator _mediator;

        public CarController(IMediator mediator)
        {
            _mediator = mediator;
        }
        #endregion

        [HttpPost]
        [SwaggerOperation(Summary = "Create a new car")]
        public async Task<IActionResult> PostCar([FromBody] CreateCarDTO model)
        {
            var result = await _mediator.Send(new CreateCarRequest(model));

            return ApiResponse.FromResult(this, result);
        }

        [HttpDelete]
        [SwaggerOperation(Summary = "Delete a car by Id")]
        public async Task<IActionResult> DeleteCar([FromBody] DeleteCarDTO model)
        {
            var result = await _mediator.Send(new DeleteCarRequest(model));
            return ApiResponse.FromResult(this, result);
        }

        [HttpPut]
        [SwaggerOperation(Summary = "Update a car")]
        public async Task<IActionResult> PutCar([FromBody] UpdateCarDTO model)
        {
            var result = await _mediator.Send(new UpdateCarRequest(model));
            return ApiResponse.FromResult(this, result);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get a car by Id")]
        public async Task<IActionResult> GetByIdCar([FromRoute] int id)
        {
            var result = await _mediator.Send(new GetByIdCarRequest(new GetByIdCarDTO { Id = id }));
            return ApiResponse.FromResult(this, result);
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get all cars")]
        public async Task<IActionResult> GetAllCars()
        {
            var result = await _mediator.Send(new GetAllCarRequest());
            return ApiResponse.FromResult(this, result);
        }

        [HttpGet("owner/{carOwnerProfileId}")]
        [SwaggerOperation(Summary = "Get all cars by CarOwnerProfileId")]
        public async Task<IActionResult> GetCarsByOwnerProfileId([FromRoute] int carOwnerProfileId)
        {
            var result = await _mediator.Send(new GetAllCarsByCarOwnerProfileIdRequest(new GetCarsByCarOwnerProfileIdDTO
            {
                CarOwnerProfileId = carOwnerProfileId
            }));

            return ApiResponse.FromResult(this, result);
        }
    }
}
