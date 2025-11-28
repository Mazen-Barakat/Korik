using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class GetAllBookingPhotoByBookingIdDTOValidator : AbstractValidator<GetAllBookingPhotoByBookingIdDTO>
    {
        private readonly IBookingService _bookingService;

        public GetAllBookingPhotoByBookingIdDTOValidator(IBookingService bookingService)
        {
            _bookingService = bookingService;

            RuleFor(x => x.BookingId)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Booking ID must be greater than 0")
                .MustAsync(async (id, CancellationToken) =>
                {
                    var exists = await _bookingService.IsExistAsync(id);
                    return exists.Success && exists.Data;
                })
                .WithMessage("The Booking does not exist.");
        }
    }
}