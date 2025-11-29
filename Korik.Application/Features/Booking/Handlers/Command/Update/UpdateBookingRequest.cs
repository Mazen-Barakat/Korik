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
    public record UpdateBookingRequest(UpdateBookingDTO model) : IRequest<ServiceResult<BookingDTO>> { }


    public class UpdateBookingRequestHandler : IRequestHandler<UpdateBookingRequest, ServiceResult<BookingDTO>>
    {
        private readonly IBookingService _bookingService;
        private readonly IValidator<UpdateBookingDTO> _validator;
        private readonly IMapper _mapper;
        public UpdateBookingRequestHandler
            (
            IBookingService bookingService,
            IValidator<UpdateBookingDTO> validator,
            IMapper mapper
            )
        {
            _bookingService = bookingService;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<ServiceResult<BookingDTO>> Handle(UpdateBookingRequest request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.model, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return ServiceResult<BookingDTO>.Fail(errors);
            }

            var bookingtoUpdate = _mapper.Map<Booking>(request.model);
            var updatedBooking = await _bookingService.UpdateAsync(bookingtoUpdate);


            if (!updatedBooking.Success)
            {
                return ServiceResult<BookingDTO>.Fail(updatedBooking.Message ?? "Failed to update booking.");
            }


            var bookingDto = _mapper.Map<BookingDTO>(updatedBooking.Data);
            return ServiceResult<BookingDTO>.Ok(bookingDto, "Booking updated successfully.");
        }
    }
}
