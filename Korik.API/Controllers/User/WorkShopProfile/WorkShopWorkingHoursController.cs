using Korik.Application;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Korik.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkShopWorkingHoursController : ControllerBase
    {
        #region Dependency Injection
        private readonly IMediator _mediator;

        public WorkShopWorkingHoursController(IMediator mediator)
        {
            _mediator = mediator;
        }
        #endregion

        #region Commands

        [HttpPost]
        [SwaggerOperation(
            Summary = "Create working hours for a workshop",
            Description = "Creates working hours entry for a specific day (Monday-Sunday) with opening/closing times. Workshop can set IsClosed=true for days off."
        )]
        public async Task<IActionResult> CreateWorkShopWorkingHours([FromBody] CreateWorkShopWorkingHoursDTO model)
        {
            var result = await _mediator.Send(new CreateWorkShopWorkingHoursRequest(model));
            return ApiResponse.FromResult(this, result);
        }

        [HttpPut]
        [SwaggerOperation(
            Summary = "Update working hours",
            Description = "Updates existing working hours entry with new times or closed status."
        )]
        public async Task<IActionResult> UpdateWorkShopWorkingHours([FromBody] UpdateWorkShopWorkingHoursDTO model)
        {
            var result = await _mediator.Send(new UpdateWorkShopWorkingHoursRequest(model));
            return ApiResponse.FromResult(this, result);
        }

        [HttpDelete("{id:int}")]
        [SwaggerOperation(
            Summary = "Delete working hours by Id",
            Description = "Deletes a specific working hours entry identified by its ID."
        )]
        public async Task<IActionResult> DeleteWorkShopWorkingHours([FromRoute] int id)
        {
            var result = await _mediator.Send(
                new DeleteWorkShopWorkingHoursRequest(
                    new DeleteWorkShopWorkingHoursDTO() { Id = id }
                ));
            return ApiResponse.FromResult(this, result);
        }

        #endregion

        #region Queries

        [HttpGet("{id:int}")]
        [SwaggerOperation(
            Summary = "Get working hours by Id",
            Description = "Retrieves a specific working hours entry by its ID."
        )]
        public async Task<IActionResult> GetWorkShopWorkingHoursById([FromRoute] int id)
        {
            var result = await _mediator.Send(
                new GetWorkShopWorkingHoursByIdRequest(
                    new GetWorkShopWorkingHoursByIdDTO() { Id = id }
                ));
            return ApiResponse.FromResult(this, result);
        }

        [HttpGet("workshop/{workshopId:int}")]
        [SwaggerOperation(
            Summary = "Get all working hours for a workshop",
            Description = "Retrieves all working hours entries for a specific workshop, showing their schedule for the entire week."
        )]
        public async Task<IActionResult> GetWorkShopWorkingHoursByWorkshopId([FromRoute] int workshopId)
        {
            var result = await _mediator.Send(
                new GetWorkShopWorkingHoursByWorkshopIdRequest(
                    new GetWorkShopWorkingHoursByWorkshopIdDTO() { WorkShopProfileId = workshopId }
                ));
            return ApiResponse.FromResult(this, result);
        }

        #endregion
    }
}