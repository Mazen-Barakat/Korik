using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class DeleteBookingPhotoByIdDTOValidator : AbstractValidator<DeleteBookingPhotoByIdDTO>
    {
        private readonly IBookingPhotoService _bookingPhotoService;

        public DeleteBookingPhotoByIdDTOValidator(IBookingPhotoService bookingPhotoService)
        {
            _bookingPhotoService = bookingPhotoService;

            RuleFor(x => x.Id)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Booking ID must be greater than 0")
                .MustAsync(async (id, CancellationToken) =>
                {
                    var exisit = await _bookingPhotoService.IsExistAsync(id);
                    return exisit.Success && exisit.Data;
                })
                .WithMessage("The Booking does not exist.");
        }
    }
}