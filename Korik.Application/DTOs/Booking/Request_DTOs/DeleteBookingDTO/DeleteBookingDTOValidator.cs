using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class DeleteBookingDTOValidator : AbstractValidator<DeleteBookingDTO>
    {
        private readonly IBookingService _bookingService;

        public DeleteBookingDTOValidator(IBookingService bookingService)
        {
            _bookingService = bookingService;

            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Booking Id must be greater than 0.")
                .MustAsync(async (id, _) =>
                {
                    var result = await _bookingService.IsExistAsync(id);
                    return result.Data;
                })
                .WithMessage("Booking does not exist.");
        }
    }

}
