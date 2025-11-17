using Korik.Application;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Korik.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        public async Task<IActionResult> CreateWorkShopPhotos([FromForm] CreateWorkShopPhotosDTO model)
        {
            var result = await _mediator.Send(new CreateWorkShopPhotosRequest(model));
            return ApiResponse.FromResult(this, result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteWorkShopPhotoById([FromRoute] int id)
        {
            var result = await _mediator.Send(new DeleteWorkShopPhotosByIdRequest(new DeleteWorkShopPhotosByIdDTO { Id = id }));
            return ApiResponse.FromResult(this, result);
        }

        #endregion Commands

        #region Queries

        [HttpGet("{workShopProfileId:int}")]
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