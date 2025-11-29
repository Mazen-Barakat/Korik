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
    public record DeleteBookingRequest(DeleteBookingDTO model) : IRequest<ServiceResult<BookingDTO>> { }


    public class DeleteBookingRequestHandler : IRequestHandler<DeleteBookingRequest, ServiceResult<BookingDTO>>
    {
        private readonly IBookingService _bookingService;
        private readonly IValidator<DeleteBookingDTO> _validator;
        private readonly IMapper _mapper;
        public DeleteBookingRequestHandler
            (
            IBookingService bookingService,
            IValidator<DeleteBookingDTO> validator,
            IMapper mapper

            )
        {
            _bookingService = bookingService;
            _validator = validator;
            _mapper = mapper;
        }
        public async Task<ServiceResult<BookingDTO>> Handle(DeleteBookingRequest request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.model, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return ServiceResult<BookingDTO>.Fail(string.Join("; ", errors));
            }
           
            
            var deleteResult = await _bookingService.DeleteAsync(request.model.Id);
            if (!deleteResult.Success || deleteResult.Data == null)
            {
                return ServiceResult<BookingDTO>.Fail(deleteResult.Message ?? "Failed to delete booking.");
            }
            
            
            var bookingDto = _mapper.Map<BookingDTO>(deleteResult.Data);
            return ServiceResult<BookingDTO>.Ok(bookingDto);
        }
    }
}
