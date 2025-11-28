using FluentValidation;
using System;

namespace Korik.Application
{
    public class UpdateReviewDTOValidator : AbstractValidator<UpdateReviewDTO>
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly ICarOwnerProfileRepository _carOwnerProfileRepository;
        private readonly IWorkShopProfileRepository _workShopProfileRepository;

        public UpdateReviewDTOValidator(
            IReviewRepository reviewRepository,
            IBookingRepository bookingRepository,
            ICarOwnerProfileRepository carOwnerProfileRepository,
            IWorkShopProfileRepository workShopProfileRepository)
        {
            _reviewRepository = reviewRepository;
            _bookingRepository = bookingRepository;
            _carOwnerProfileRepository = carOwnerProfileRepository;
            _workShopProfileRepository = workShopProfileRepository;

            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Id must be greater than 0.")
                .MustAsync(async (id, cancellation) => await _reviewRepository.IsExistAsync(id))
                .WithMessage("Review with the given Id does not exist.");

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

            RuleFor(x => x.CreatedAt)
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("CreatedAt cannot be a future date.");


            RuleFor(x => x.BookingId)
                .GreaterThan(0)
                .WithMessage("Booking ID must be greater than 0.")
                .MustAsync(async (id, cancellation) => await _bookingRepository.IsExistAsync(id))
                .WithMessage("Booking ID does not exist.");

            RuleFor(x => x.CarOwnerProfileId)
                .GreaterThan(0)
                .WithMessage("Car Owner Profile ID must be greater than 0.")
                .MustAsync(async (id, cancellation) => await _carOwnerProfileRepository.IsExistAsync(id))
                .WithMessage("Car Owner Profile ID does not exist.");

            RuleFor(x => x.WorkShopProfileId)
                .GreaterThan(0)
                .WithMessage("WorkShop Profile ID must be greater than 0.")
                .MustAsync(async (id, cancellation) => await _workShopProfileRepository.IsExistAsync(id))
                .WithMessage("WorkShop Profile ID does not exist.");
        }
    }
}
