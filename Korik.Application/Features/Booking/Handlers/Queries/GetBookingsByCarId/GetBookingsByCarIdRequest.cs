using AutoMapper;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public record GetBookingsByCarIdRequest(GetBookingsByCarIdDTO model) : IRequest<ServiceResult<IEnumerable<BookingDTO>>>{ }



    public class GetBookingsByCarIdRequestHandler : IRequestHandler<GetBookingsByCarIdRequest, ServiceResult<IEnumerable<BookingDTO>>>
    {
        private readonly IBookingService _bookingService;
        private readonly IValidator<GetBookingsByCarIdDTO> _validator;
        private readonly IMapper _mapper;
        public GetBookingsByCarIdRequestHandler
            (
            IBookingService bookingService,
            IValidator<GetBookingsByCarIdDTO> validator,
            IMapper mapper
            )
        {
            _bookingService = bookingService;
            _validator = validator;
            _mapper = mapper;
        }
        public async Task<ServiceResult<IEnumerable<BookingDTO>>> Handle(GetBookingsByCarIdRequest request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.model, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage);
                var errorMessage = string.Join(", ", errors);
                return ServiceResult<IEnumerable<BookingDTO>>.Fail(errorMessage);
            }


            var bookingsResult = await _bookingService.GetBookingsByCarIdAsync(request.model.CarId);
            if (!bookingsResult.Success)
            {
                return ServiceResult<IEnumerable<BookingDTO>>.Fail(bookingsResult.Message ?? "Failed to retrieve bookings.");
            }

            var bookingDTOs = _mapper.Map<IEnumerable<BookingDTO>>(bookingsResult.Data);
            return ServiceResult<IEnumerable<BookingDTO>>.Ok(bookingDTOs, "Bookings retrieved successfully.");

        }
    }

}
