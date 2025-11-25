using Korik.Application;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace Korik.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkshopServiceController : ControllerBase
    {
        #region Dependency Injection

        private readonly IMediator _mediator;

        public WorkshopServiceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        #endregion Dependency Injection

        #region Commands

        [HttpPost]
        [Authorize(Roles = "WORKSHOP,ADMIN")]
        [SwaggerOperation(
            Summary = "Create a workshop service offering",
            Description = "Creates a new service offering for a workshop, including duration, pricing (min/max), and car origin (European, Asian, American, General). Only accessible by workshop owners and admins."
        )]
        public async Task<IActionResult> CreateWorkshopService([FromBody] CreateWorkshopServiceDTO model)
        {
            var result = await _mediator.Send(new CreateWorkshopServiceRequest(model));
            return ApiResponse.FromResult(this, result);
        }

        [HttpPut]
        [Authorize(Roles = "WORKSHOP,ADMIN")]
        [SwaggerOperation(
            Summary = "Update a workshop service offering",
            Description = "Updates an existing workshop service. Supports partial updates - only send the fields you want to change. Fields not provided will remain unchanged."
        )]
        public async Task<IActionResult> UpdateWorkshopService([FromBody] UpdateWorkshopServiceDTO model)
        {
            var result = await _mediator.Send(new UpdateWorkshopServiceRequest(model));
            return ApiResponse.FromResult(this, result);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "WORKSHOP,ADMIN")]
        [SwaggerOperation(
            Summary = "Delete a workshop service offering",
            Description = "Deletes a workshop service offering by ID. This will permanently remove the service from the workshop's offerings."
        )]
        public async Task<IActionResult> DeleteWorkshopService([FromRoute] int id)
        {
            var result = await _mediator.Send(new DeleteWorkshopServiceRequest(new DeleteWorkshopServiceDTO { Id = id }));
            return ApiResponse.FromResult(this, result);
        }

        #endregion Commands

        #region Queries

        [HttpGet("{id:int}")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Get workshop service by ID",
            Description = "Retrieves detailed information about a specific workshop service offering including duration, pricing, and origin."
        )]
        public async Task<IActionResult> GetWorkshopServiceById([FromRoute] int id)
        {
            var result = await _mediator.Send(new GetWorkshopServiceByIdRequest(new GetWorkshopServiceByIdDTO { Id = id }));
            return ApiResponse.FromResult(this, result);
        }

        [HttpGet("Search-Workshops-By-Service-And-Origin")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Search workshops by service and car origin",
            Description = "Returns a paginated list of workshops that offer a specific service for a specific car origin (e.g., all workshops offering 'Oil Change' for 'European' cars). Includes workshop details, location, rating, and pricing information."
        )]
        public async Task<IActionResult> GetWorkshopsByServiceAndOrigin([FromQuery] SearchWorkshopsByServiceAndOriginDTO model)
        {
            var result = await _mediator.Send(new SearchWorkshopsByServiceAndOriginRequest(model));
            return ApiResponse.FromResult(this, result);
        }

        #endregion Queries
    }
}