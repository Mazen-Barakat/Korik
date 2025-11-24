using Korik.Application;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Korik.API.Controllers.Service
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ServiceController(IMediator mediator)
        {
            _mediator = mediator;
        }


        #region Commands
        [HttpPost]
        [SwaggerOperation(
            Summary = "Create a new Service",
            Description = "Creates a new service using the provided data."
        )]
        public async Task<IActionResult> PostService([FromBody] CreateServiceDTO model)
        {
            var result = await _mediator.Send(new CreateServiceRequest(model));
            return ApiResponse.FromResult(this, result);
        }



        [HttpPut]
        [SwaggerOperation(
            Summary = "Update an existing Service",
            Description = "Updates an existing service's details using the provided data."
        )]
        public async Task<IActionResult> PutService([FromBody] UpdateServiceDTO model)
        {
            var result = await _mediator.Send(new UpdateServiceRequest(model));
            return ApiResponse.FromResult(this, result);
        }
        #endregion


        #region Queries

        #endregion


    }
}
