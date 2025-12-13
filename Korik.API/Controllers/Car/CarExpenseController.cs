using Korik.Application;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace Korik.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarExpenseController : ControllerBase
    {
        #region Dependency Injection

        private readonly IMediator _mediator;
        private readonly ICarService _carService;
        private readonly ICarOwnerProfileService _carOwnerProfileService;

        public CarExpenseController(IMediator mediator, ICarService carService, ICarOwnerProfileService carOwnerProfileService)
        {
            _mediator = mediator;
            _carService = carService;
            _carOwnerProfileService = carOwnerProfileService;
        }

        #endregion Dependency Injection

        #region Commands

        [HttpPost]
        [SwaggerOperation(Summary = "Create a new car expense")]
        public async Task<IActionResult> PostCarExpense([FromBody] CreateCarExpanseDTO model)
        {


            var result = await _mediator.Send(new CreateCarExpenseRequest(model));
            return ApiResponse.FromResult(this, result);
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete a car expense by Id")]
        public async Task<IActionResult> DeleteCarExpense([FromRoute] int id)
        {
            var result = await _mediator.Send(new DeleteCarExpenseRequest(new DeleteCarExpenseDTO { Id = id }));
            return ApiResponse.FromResult(this, result);
        }



        [HttpPut]
        [SwaggerOperation(Summary = "Update an existing car expense")]
        public async Task<IActionResult> PutCarExpense([FromBody] UpdateCarExpenseDTO model)
        {
            var result = await _mediator.Send(new UpdateCarExpenseRequest(model));
            return ApiResponse.FromResult(this, result);
        }

        #endregion Commands

        #region Queries

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get a car expense by Id")]
        public async Task<IActionResult> GetCarExpenseById([FromRoute] int id)
        {
            var result = await _mediator.Send(new GetByIdCarExpenseRequest(new GetByIdCarExpenseDTO { Id = id }));
            return ApiResponse.FromResult(this, result);
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get all car expenses")]
        public async Task<IActionResult> GetAllCarExpenses()
        {
            var result = await _mediator.Send(new GetAllCarExpenseRequest());
            return ApiResponse.FromResult(this, result);
        }

        [HttpGet("ByCarId/{carId}")]
        [SwaggerOperation(Summary = "Get all car expenses by Car Id")]
        public async Task<IActionResult> GetAllCarExpensesByCarId([FromRoute] int carId)
        {
            var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var carOwnerProfileResult = await _carOwnerProfileService.GetByApplicationUserIdAsync(applicationUserId);
            var carResult = await _carService.GetByIdAsync(carId);
            if (carOwnerProfileResult.Success && carResult.Success)
            {
                if (carResult.Data.CarOwnerProfileId != carOwnerProfileResult.Data.Id)
                {
                    return Forbid("You do not have permission to access this car.");
                }
            }

            var result = await _mediator.Send(new GetAllCarExpensesByCarIdRequest(new GetAllCarExpensesByCarIdDTO { CarId = carId }));
            return ApiResponse.FromResult(this, result);
        }

        #endregion    
    }
}