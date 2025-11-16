using Korik.Application;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Korik.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubcategoryController : ControllerBase
    {
        #region Dependency Injection
        private readonly IMediator _mediator;
        public SubcategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        #endregion

        #region Commands
      
        [HttpPost]
        [SwaggerOperation (
            Summary = "Create a new Subcategory",
            Description = "Creates a new category using the provided data such as name, Description, and CategoryId."
        )]
        public async Task<IActionResult> PostSubcategory([FromBody] CreateSubcategoryDTO model)
        {
            var result = await _mediator.Send(new CreateSubcategoryRequest(model));

            return ApiResponse.FromResult(this, result);
        }



        [HttpPut]
        [SwaggerOperation(
            Summary = "Update an existing Subcategory",
            Description = "Updates an existing subcategory identified by its ID with the provided data such as name, Description, and CategoryId."
        )]
        public async Task<IActionResult> PutSubcategory([FromBody] UpdateSubcategoryDTO model)
        {
            var result = await _mediator.Send(new UpdateSubcategoryRequest(model));

            return ApiResponse.FromResult(this, result);
        }

        #endregion



        #region Queries

        #endregion


    }
}
