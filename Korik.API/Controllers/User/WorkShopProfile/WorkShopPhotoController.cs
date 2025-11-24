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
    [Authorize(Roles = "WORKSHOP")]
    public class WorkShopPhotoController : ControllerBase
    {
        #region Dependency Injection

        private readonly IMediator _mediator;

        public WorkShopPhotoController(IMediator mediator)
        {
            _mediator = mediator;
        }

        #endregion Dependency Injection

        #region Commands

        [HttpPost]
        [SwaggerOperation(
            Summary = "Create workshop photos",
            Description = "Uploads one or multiple photos and associates them with a workshop profile. "
                        + "This endpoint accepts form-data containing images and stores them in the system."
        )]
        public async Task<IActionResult> CreateWorkShopPhotos([FromForm] CreateWorkShopPhotosDTO model)
        {
            var result = await _mediator.Send(new CreateWorkShopPhotosRequest(model));
            return ApiResponse.FromResult(this, result);
        }

        [HttpDelete("{id:int}")]
        [SwaggerOperation(
            Summary = "Delete a workshop photo by ID",
            Description = "Deletes a specific workshop photo using its unique ID. "
                        + "Useful for removing outdated or incorrect workshop gallery images."
        )]
        public async Task<IActionResult> DeleteWorkShopPhotoById([FromRoute] int id)
        {
            var result = await _mediator.Send(new DeleteWorkShopPhotosByIdRequest(new DeleteWorkShopPhotosByIdDTO { Id = id }));
            return ApiResponse.FromResult(this, result);
        }

        #endregion Commands

        #region Queries

        [HttpGet("{workShopProfileId:int}")]
        [SwaggerOperation(
            Summary = "Get all photos for a workshop",
            Description = "Retrieves all photos associated with a specific workshop profile ID. "
                        + "This is typically used to display a photo gallery for a workshop."
        )]
        public async Task<IActionResult> GetAllWorkShopPhotoByWWorkShopId([FromRoute] int workShopProfileId)
        {
            var result = await _mediator.Send(new GetAllWorkShopPhotosByWorkShopIdRequest(
                new GetAllWorkShopPhotosByWorkShopIdDTO { WorkShopProfileId = workShopProfileId }
                ));
            return ApiResponse.FromResult(this, result);
        }

        #endregion Queries
    }
}