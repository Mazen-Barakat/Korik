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
    [Authorize(Roles = "CAROWNER")]
    public class BookingPhotoController : ControllerBase
    {
        #region Dependency Injection

        private readonly IMediator _mediator;

        public BookingPhotoController(IMediator mediator)
        {
            _mediator = mediator;
        }

        #endregion Dependency Injection

        #region Commands

        [HttpPost]
        [SwaggerOperation(
            Summary = "Create Booking photos",
            Description = "Uploads one or multiple photos and associates them with a Booking. "
                        + "This endpoint accepts form-data containing images and stores them in the system."
        )]
        public async Task<IActionResult> CreateBookingPhotos([FromForm] CreateBookingPhotoDTO model)
        {
            var result = await _mediator.Send(new CreateBookingPhotoRequest(model));
            return ApiResponse.FromResult(this, result);
        }

        [HttpDelete("{id:int}")]
        [SwaggerOperation(
            Summary = "Delete a Booking photo by ID",
            Description = "Deletes a specific Booking photo using its unique ID. "
                        + "Useful for removing outdated or incorrect workshop gallery images."
        )]
        public async Task<IActionResult> DeleteBookingPhotoById([FromRoute] int id)
        {
            var result = await _mediator.Send(new DeleteBookingPhotoRequest(new DeleteBookingPhotoByIdDTO { Id = id }));
            return ApiResponse.FromResult(this, result);
        }

        #endregion Commands

        #region Queries

        [HttpGet("{bookingId:int}")]
        [SwaggerOperation(
            Summary = "Get all photos for a Booking",
            Description = "Retrieves all photos associated with a specific Booking ID. "
                        + "This is typically used to display a photo gallery for a Booking."
        )]
        public async Task<IActionResult> GetAllWorkShopPhotoByWWorkShopId([FromRoute] int bookingId)
        {
            var result = await _mediator.Send(new GetAllBookingPhotoByBookingIdRequest(
                new GetAllBookingPhotoByBookingIdDTO { BookingId = bookingId }
                ));
            return ApiResponse.FromResult(this, result);
        }

        #endregion Queries
    }
}