using FluentValidation;
using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class CreateReviewDTOValidator : AbstractValidator<CreateReviewDTO>
    {
        private readonly IGenericRepository<Booking> _bookingRepository;
        private readonly ICarOwnerProfileRepository _carOwnerProfileRepository;
        private readonly IWorkShopProfileRepository _workShopProfileRepository;

        public CreateReviewDTOValidator(
            IGenericRepository<Booking> bookingRepository,
            ICarOwnerProfileRepository carOwnerProfileRepository,
            IWorkShopProfileRepository workShopProfileRepository)
        {
            _bookingRepository = bookingRepository;
            _carOwnerProfileRepository = carOwnerProfileRepository;
            _workShopProfileRepository = workShopProfileRepository;

            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5)
                .WithMessage("Rating must be between 1 and 5.");

            RuleFor(x => x.Comment)
                .NotEmpty()
                .WithMessage("Comment is required.")
                .MaximumLength(500)
                .WithMessage("Comment cannot exceed 500 characters.");

            RuleFor(x => x.PaidAmount)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Paid amount must be greater than or equal to 0.");

            RuleFor(x => x.BookingId)
                .MustAsync(async (id, cancellation) => await _bookingRepository.IsExistAsync(id))
                .WithMessage("Booking ID does not exist.");

            RuleFor(x => x.CarOwnerProfileId)
                .MustAsync(async (id, cancellation) => await _carOwnerProfileRepository.IsExistAsync(id))
                .WithMessage("Car Owner Profile ID does not exist.");

            RuleFor(x => x.WorkShopProfileId)
                .MustAsync(async (id, cancellation) => await _workShopProfileRepository.IsExistAsync(id))
                .WithMessage("WorkShop Profile ID does not exist.");
        }
    }
}
