using AutoMapper;
using FluentValidation;
using Korik.Domain;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Korik.Application
{
    public record GetCarOwnerProfileByBookingIdRequest(GetCarOwnerProfileByBookingIdDTO Model) : IRequest<ServiceResult<BookingCarOwnerProfileDTO>> { }

    public class GetCarOwnerProfileByBookingIdRequestHandler : IRequestHandler<GetCarOwnerProfileByBookingIdRequest, ServiceResult<BookingCarOwnerProfileDTO>>
    {
        private readonly IBookingService _bookingService;
        private readonly IValidator<GetCarOwnerProfileByBookingIdDTO> _validator;
        private readonly IMapper _mapper;

        public GetCarOwnerProfileByBookingIdRequestHandler(
            IBookingService bookingService,
            IValidator<GetCarOwnerProfileByBookingIdDTO> validator,
            IMapper mapper)
        {
            _bookingService = bookingService;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<ServiceResult<BookingCarOwnerProfileDTO>> Handle(GetCarOwnerProfileByBookingIdRequest request, CancellationToken cancellationToken)
        {
            // Validate request
            var validationResult = await _validator.ValidateAsync(request.Model, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage);
                var errorMessage = string.Join(", ", errors);
                return ServiceResult<BookingCarOwnerProfileDTO>.Fail(errorMessage);
            }

            // Get booking with Car and CarOwnerProfile navigation properties
            var bookingResult = await _bookingService.GetByIdWithIncludeAsync(
                request.Model.BookingId,
                b => b.Car.CarOwnerProfile
            );

            if (!bookingResult.Success || bookingResult.Data == null)
            {
                return ServiceResult<BookingCarOwnerProfileDTO>.Fail("Booking not found.");
            }

            var carOwnerProfile = bookingResult.Data.Car?.CarOwnerProfile;

            if (carOwnerProfile == null)
            {
                return ServiceResult<BookingCarOwnerProfileDTO>.Fail("Car owner profile not found for this booking.");
            }

            // Map to DTO
            var carOwnerProfileDto = _mapper.Map<BookingCarOwnerProfileDTO>(carOwnerProfile);
            carOwnerProfileDto.FullName = $"{carOwnerProfile.FirstName} {carOwnerProfile.LastName}";

            return ServiceResult<BookingCarOwnerProfileDTO>.Ok(carOwnerProfileDto, "Car owner profile retrieved successfully.");
        }
    }
}
