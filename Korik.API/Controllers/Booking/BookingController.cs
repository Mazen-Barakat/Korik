using Korik.Application;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Korik.API.Controllers.Booking
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        #region Dependency Injection
        private readonly IMediator _mediator;
        public BookingController(IMediator mediator)
        {
            _mediator = mediator;
        }
        #endregion


        #region Commands
        [HttpPost]
        [SwaggerOperation(
            Summary = "Create a new booking",
            Description = "Creates a new booking with the provided details."
        )]
        public async Task<IActionResult> PostBooking([FromBody] CreateBookingDTO model)
        {
            var result = await _mediator.Send(new CreateBookingRequest(model));
           
            return ApiResponse.FromResult(this, result);
        }
        #endregion


        #region Queries

        #endregion

    }
}
