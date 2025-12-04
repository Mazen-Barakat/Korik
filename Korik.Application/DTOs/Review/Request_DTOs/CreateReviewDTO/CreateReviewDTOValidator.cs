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
        private readonly IBookingService _bookingService;
        private readonly ICarOwnerProfileService _carOwnerProfileService;
        private readonly IWorkShopProfileService _workShopProfileService;

        public CreateReviewDTOValidator(
            IBookingService bookingService,
            ICarOwnerProfileService carOwnerProfileService,
            IWorkShopProfileService workShopProfileService)
        {
            _bookingService = bookingService;
            _carOwnerProfileService = carOwnerProfileService;
            _workShopProfileService = workShopProfileService;

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
                .MustAsync(async (id, cancellation) =>
                {
                    var bookingWithReviewResult = await _bookingService.GetByIdWithIncludeAsync(id, b => b.Review);
                    if(bookingWithReviewResult.Success && bookingWithReviewResult.Data.Review != null)
                        return false;
                    return true;
                })
                .WithMessage("Booking has already a review.");

            RuleFor(x => x.CarOwnerProfileId)
                .MustAsync(async (id, cancellation) => 
                {
                    var result = await _carOwnerProfileService.IsExistAsync(id);
                    return result.Data;
                })
                .WithMessage("Car Owner Profile ID does Already exist.");

            RuleFor(x => x.WorkShopProfileId)
                .MustAsync(async (id, cancellation) =>
                {
                    var result = await _workShopProfileService.IsExistAsync(id);
                    return result.Data;
                })
                .WithMessage("WorkShop Profile ID does Already exist.");
        }
    }
}
