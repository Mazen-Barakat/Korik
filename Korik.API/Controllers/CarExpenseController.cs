using Korik.Application;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Korik.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarExpenseController : ControllerBase
    {
        #region Dependency Injection

        private readonly IMediator _mediator;

        public CarExpenseController(IMediator mediator)
        {
            _mediator = mediator;
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

        [HttpDelete]
        [SwaggerOperation(Summary = "Delete a car expense by Id")]
        public async Task<IActionResult> DeleteCarExpense([FromBody] DeleteCarExpenseDTO model)
        {
            var result = await _mediator.Send(new DeleteCarExpenseRequest(model));
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

        #endregion Queries

    }
}