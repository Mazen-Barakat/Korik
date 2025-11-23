using Korik.Application;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Korik.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        #region Dependency Injection
        private readonly IMediator _mediator;

        public ServiceController(IMediator mediator)
        {
            _mediator = mediator;
        }
        #endregion

        #region Commands

        [HttpPost]
        [SwaggerOperation(
            Summary = "Create a new service",
            Description = "Creates a new service with name, description, duration, price range, image URL, and subcategory."
        )]
        public async Task<IActionResult> CreateService([FromBody] CreateServiceDTO model)
        {
            var result = await _mediator.Send(new CreateServiceRequest(model));
            return ApiResponse.FromResult(this, result);
        }

        [HttpPut]
        [SwaggerOperation(
            Summary = "Update an existing service",
            Description = "Updates service details including name, description, duration, prices, image, and subcategory."
        )]
        public async Task<IActionResult> UpdateService([FromBody] UpdateServiceDTO model)
        {
            var result = await _mediator.Send(new UpdateServiceRequest(model));
            return ApiResponse.FromResult(this, result);
        }

        [HttpDelete("{id:int}")]
        [SwaggerOperation(
            Summary = "Delete a service by ID",
            Description = "Deletes a specific service identified by its ID."
        )]
        public async Task<IActionResult> DeleteService([FromRoute] int id)
        {
            var result = await _mediator.Send(
                new DeleteServiceRequest(
                    new DeleteServiceDTO() { Id = id }
                ));
            return ApiResponse.FromResult(this, result);
        }

        #endregion

        #region Queries

        [HttpGet("{id:int}")]
        [SwaggerOperation(
            Summary = "Get service by ID",
            Description = "Retrieves a specific service by its ID, including subcategory details."
        )]
        public async Task<IActionResult> GetServiceById([FromRoute] int id)
        {
            var result = await _mediator.Send(
                new GetServiceByIdRequest(
                    new GetServiceByIdDTO() { Id = id }
                ));
            return ApiResponse.FromResult(this, result);
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Get all services",
            Description = "Retrieves all services with their subcategory information."
        )]
        public async Task<IActionResult> GetAllServices()
        {
            var result = await _mediator.Send(new GetAllServicesRequest());
            return ApiResponse.FromResult(this, result);
        }

        [HttpGet("subcategory/{subcategoryId:int}")]
        [SwaggerOperation(
            Summary = "Get services by subcategory ID",
            Description = "Retrieves all services belonging to a specific subcategory."
        )]
        public async Task<IActionResult> GetServicesBySubcategoryId([FromRoute] int subcategoryId)
        {
            var result = await _mediator.Send(
                new GetServicesBySubcategoryIdRequest(
                    new GetServicesBySubcategoryIdDTO() { SubcategoryId = subcategoryId }
                ));
            return ApiResponse.FromResult(this, result);
        }

        #endregion
    }
}