using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class CreateBookingPhotoDTOValidator : AbstractValidator<CreateBookingPhotoDTO>
    {
        public CreateBookingPhotoDTOValidator()
        {
            //RuleFor(x => x.BookingId)
            //    .GreaterThanOrEqualTo(1)
            //    .WithMessage("Booking ID must be greater than 0")
            //    .MustAsync(async (id, CancellationToken) =>
            //    {
            //        var exists = await _bookingPhotoService.IsExistAsync(id);
            //        return exists.Success && exists.Data;
            //    })
            //    .WithMessage("The Booking does not exist.");

            RuleFor(x => x.Photos)
                .NotNull()
                .WithMessage("Photos are required")
                .NotEmpty()
                .WithMessage("At least one photo is required")
                .Must(photos => photos != null && photos.Count > 0)
                .WithMessage("Photos list cannot be empty")
                .Must(photos => photos.Count <= 3)
                .WithMessage("Maximum 10 photos allowed");

            RuleForEach(x => x.Photos)
                .ChildRules(photo =>
                {
                    photo.RuleFor(f => f.Length)
                        .NotNull()
                        .LessThanOrEqualTo(5 * 1024 * 1024) // 5MB max per file
                        .WithMessage("Each photo must be less than 5MB");

                    photo.RuleFor(f => f.ContentType)
                        .NotNull()
                        .Must(contentType =>
                            contentType.Equals("image/jpeg", StringComparison.OrdinalIgnoreCase) ||
                            contentType.Equals("image/jpg", StringComparison.OrdinalIgnoreCase) ||
                            contentType.Equals("image/png", StringComparison.OrdinalIgnoreCase) ||
                            contentType.Equals("image/webp", StringComparison.OrdinalIgnoreCase))
                        .WithMessage("Only JPEG, PNG, and WebP images are allowed");

                    photo.RuleFor(f => f.FileName)
                        .NotEmpty()
                        .WithMessage("File name is required");
                });
        }
    }
}