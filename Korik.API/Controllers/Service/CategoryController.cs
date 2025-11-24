using Korik.Application;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Korik.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IMediator _mediator;
        public CategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }


        #region Commands
        [HttpPost]
        [SwaggerOperation(
            Summary = "Create a new category",
            Description = "Creates a new category using the provided data such as name, icon URL, and display order."
        )]
        public async Task<IActionResult> PostCategory([FromBody] CreateCategoryDTO model)
        {
            var result = await _mediator.Send(new CreateCategoryRequest(model));

            return ApiResponse.FromResult(this, result);
        }


        [HttpPut]
        [SwaggerOperation(
            Summary = "Update an existing category",
            Description = "Updates an existing category's details such as name, icon URL, and display order using the provided data."
        )]
        public async Task<IActionResult> PutCategory([FromBody] UpdateCategoryDTO model)
        {
            var result = await _mediator.Send(new UpdateCategoryRequest(model));
            return ApiResponse.FromResult(this, result);
        }


        [HttpDelete("{id:int}")]
        [SwaggerOperation(
            Summary = "Delete a category by Id",
            Description = "Deletes the category identified by the provided Id."
        )]
        public async Task<IActionResult> DeleteCategory([FromRoute] int id)
        {
            var result = await _mediator.Send(
                new DeleteCategoryRequest(
                    new DeleteCategoryDTO() { Id = id}
                    ));

            return ApiResponse.FromResult(this, result);
        }

        #endregion



        #region Qieries
        [HttpGet]
        [SwaggerOperation(
            Summary = "Get all categories",
            Description = "Retrieves a list of all categories."
        )]
        public async Task<IActionResult> GetAllCategories()
        {
            var result = await _mediator.Send(new GetAllCategoriesRequest());
            return ApiResponse.FromResult(this, result);
        }



        [HttpGet("{id:int}")]
        [SwaggerOperation(
            Summary = "Get a category by Id",
            Description = "Retrieves the category identified by the provided Id."
        )]
        public async Task<IActionResult> GetCategoryById([FromRoute] int id)
        {
            var result = await _mediator.Send(
                new GetCategoryByIdRequest(
                    new GetCategoryByIdDTO() { Id = id}
                    ));

            return ApiResponse.FromResult(this, result);
        }
        #endregion
    }
}
