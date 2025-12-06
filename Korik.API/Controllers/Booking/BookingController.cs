using Korik.Application;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using System.Threading.Tasks;

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

        #endregion Dependency Injection

        #region Commands

        [HttpPost]
        [SwaggerOperation(
            Summary = "Create a new booking",
            Description = "Creates a new booking with the provided details."
        )]
        public async Task<IActionResult> PostBooking([FromForm] CreateBookingDTO model)
        {
            var result = await _mediator.Send(new CreateBookingRequest(model));

            return ApiResponse.FromResult(this, result);
        }

        [HttpPut]
        [SwaggerOperation(
            Summary = "Update an existing booking",
            Description = "Updates the details of an existing booking."
        )]
        public async Task<IActionResult> PutBooking([FromBody] UpdateBookingDTO model)
        {
            var result = await _mediator.Send(new UpdateBookingRequest(model));
            return ApiResponse.FromResult(this, result);
        }

        [HttpPut("Update-Booking-Status")]
        [SwaggerOperation(
            Summary = "Update an existing booking Status",
            Description = "Updates the booking Statu of an existing booking."
        )]
        public async Task<IActionResult> UpdateBookingStatus([FromBody] UpdateBookingStatusDTO model)
        {
            // Get user ID from JWT token
            var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            model.ApplicationUserId = applicationUserId;

            var result = await _mediator.Send(new UpdateBookingStatusRequest(model));
            return ApiResponse.FromResult(this, result);
        }

        [HttpPut("{bookingId:int}/response")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Update booking response status",
            Description = "Changes the workshop's response to a booking (Accept/Decline). Includes validation for business rules."
        )]
        public async Task<IActionResult> UpdateBookingResponse([FromRoute] int bookingId, [FromBody] UpdateBookingResponseDTO model)
        {
            var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _mediator.Send(new UpdateBookingResponseRequest(bookingId, model, applicationUserId!));
            return ApiResponse.FromResult(this, result);
        }

        [HttpPost("confirm-appointment")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Confirm or decline appointment arrival",
            Description = "Allows car owner or workshop owner to confirm or decline appointment. Both parties must confirm for booking to proceed to InProgress status."
        )]
        public async Task<IActionResult> ConfirmAppointment([FromBody] ConfirmAppointmentDTO model)
        {
            // Get user ID from JWT token
            var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            model.ApplicationUserId = applicationUserId!;

            var result = await _mediator.Send(new ConfirmAppointmentRequest(model));
            return ApiResponse.FromResult(this, result);
        }

        [HttpDelete("{id:int}")]
        [SwaggerOperation(
            Summary = "Delete a booking by Id",
            Description = "Deletes the booking identified by the provided Id."
        )]
        public async Task<IActionResult> DeleteBooking([FromRoute] int id)
        {
            var result = await _mediator.Send(
                new DeleteBookingRequest(
                    new DeleteBookingDTO() { Id = id }
                    ));
            return ApiResponse.FromResult(this, result);
        }

        #endregion Commands

        #region Queries

        [HttpGet("{bookingId:int}/details")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Get enhanced booking details",
            Description = "Retrieves complete booking information including customer, vehicle, service details, and response status."
        )]
        public async Task<IActionResult> GetEnhancedBookingDetails([FromRoute] int bookingId)
        {
            var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _mediator.Send(new GetEnhancedBookingDetailsRequest(bookingId, applicationUserId!));
            return ApiResponse.FromResult(this, result);
        }

        [HttpGet("{bookingId:int}/time-status")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Get booking time status",
            Description = "Returns precise timing information for a booking, including seconds until appointment time."
        )]
        public async Task<IActionResult> GetBookingTimeStatus([FromRoute] int bookingId)
        {
            var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _mediator.Send(new GetBookingTimeStatusRequest(bookingId, applicationUserId!));
            return ApiResponse.FromResult(this, result);
        }

        [HttpGet("{bookingId:int}/confirmation-status")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Get booking confirmation status",
            Description = "Returns the confirmation status for both car owner and workshop owner."
        )]
        public async Task<IActionResult> GetConfirmationStatus([FromRoute] int bookingId)
        {
            var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _mediator.Send(new GetConfirmationStatusRequest(bookingId, applicationUserId!));
            return ApiResponse.FromResult(this, result);
        }

        [HttpGet("ByCar")]
        [SwaggerOperation(
            Summary = "Get bookings by Car Id",
            Description = "Retrieves all bookings associated with the specified Car Id."
        )]
        public async Task<IActionResult> GetBookingsByCarId([FromQuery] GetBookingsByCarIdDTO model)
        {
            var result = await _mediator.Send(new GetBookingsByCarIdRequest(model));
            return ApiResponse.FromResult(this, result);
        }

        [HttpGet("ByWorkshop/{workshopProfileId:int}")]
        [SwaggerOperation(
            Summary = "Get bookings by Workshop Id",
            Description = "Retrieves all bookings associated with the specified Workshop Id."
        )]
        public async Task<IActionResult> GetBookingsByWorkshopId([FromRoute] int workshopProfileId)
        {
            var result = await _mediator.Send(
                new GetBookingsByWorkshopProfileIdRequest(
                    new GetBookingsByWorkshopProfileIdDTO() { WorkshopProfileId = workshopProfileId }
                    ));
            return ApiResponse.FromResult(this, result);
        }

        [HttpGet("{id:int}")]
        [SwaggerOperation(
            Summary = "Get a booking by Id",
            Description = "Retrieves the booking identified by the provided Id."
        )]
        public async Task<IActionResult> GetBookingById([FromRoute] int id)
        {
            var result = await _mediator.Send(
                new GetBookingByIdRequest(
                    new GetBookingByIdDTO() { Id = id }
                    ));
            return ApiResponse.FromResult(this, result);
        }

        [HttpGet("Get-Booking-Services-With-Review-By-CarId/{CarId:int}")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Get completed booking services with reviews by Car ID",
            Description = "Retrieves all completed bookings with their associated service details and reviews for a specific car. "
                        + "Only returns bookings with 'Completed' status. Useful for displaying service history and customer reviews."
        )]
        public async Task<IActionResult> GetBookingServicesWithReview([FromRoute] GetBookingServicesWithReviewByCarIdDTO model)
        {
            var result = await _mediator.Send(new GetBookingServicesWithReviewByCarIdRequest(model));

            return ApiResponse.FromResult(this, result);
        }

        #endregion Queries
    }
}