using AutoMapper;
using FluentValidation;
using Korik.Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public record GetBookingsByWorkshopProfileIdRequest(GetBookingsByWorkshopProfileIdDTO model) 
        : IRequest<ServiceResult<IEnumerable<BookingDTO>>> { }


    public class GetBookingsByWorkshopProfileIdRequestHandler 
        : IRequestHandler<GetBookingsByWorkshopProfileIdRequest, ServiceResult<IEnumerable<BookingDTO>>>
    {
        private readonly IBookingService _bookingService;
        private readonly IValidator<GetBookingsByWorkshopProfileIdDTO> _validator;
        private readonly IMapper _mapper;
        public GetBookingsByWorkshopProfileIdRequestHandler
            (
            IBookingService bookingService,
            IValidator<GetBookingsByWorkshopProfileIdDTO> validator,
            IMapper mapper
            )
        {
            _bookingService = bookingService;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<ServiceResult<IEnumerable<BookingDTO>>> Handle(GetBookingsByWorkshopProfileIdRequest request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.model, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(",", validationResult.Errors.Select(e => e.ErrorMessage));
                return ServiceResult<IEnumerable<BookingDTO>>.Fail(errors);
            }

            var result = await _bookingService.GetBookingsByWorkshopProfileIdAsync(request.model.WorkshopProfileId);
            if (!result.Success)
            {
                return ServiceResult<IEnumerable<BookingDTO>>.Fail(result.Message ?? "Failed to retrieve bookings.");
            }

            var bookingDTOs = _mapper.Map<IEnumerable<BookingDTO>>(result.Data);
            return ServiceResult<IEnumerable<BookingDTO>>.Ok(bookingDTOs);
        }
    }
}
