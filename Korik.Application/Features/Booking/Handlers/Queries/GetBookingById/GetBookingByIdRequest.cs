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
    public record GetBookingByIdRequest(GetBookingByIdDTO Model) : IRequest<ServiceResult<BookingDTO>> { }


    public class GetBookingByIdRequestHandler : IRequestHandler<GetBookingByIdRequest, ServiceResult<BookingDTO>>
    {
        #region Dependency Injection
        private readonly IBookingService _bookingService;
        private readonly IMapper _mapper;
        private readonly IValidator<GetBookingByIdDTO> _validator;
        public GetBookingByIdRequestHandler
            (
            IBookingService bookingService,
            IMapper mapper,
            IValidator<GetBookingByIdDTO> validator
            )
        {
            _bookingService = bookingService;
            _mapper = mapper;
            _validator = validator;
        }
        #endregion Dependency Injection

        public async Task<ServiceResult<BookingDTO>> Handle(GetBookingByIdRequest request, CancellationToken cancellationToken)
        {
           var validationResult = await _validator.ValidateAsync(request.Model, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return ServiceResult<BookingDTO>.Fail(string.Join(", ", errors));
            }
            
            
            var bookingResult = await _bookingService.GetByIdAsync(request.Model.Id);
            if (!bookingResult.Success || bookingResult.Data == null)
            {
                return ServiceResult<BookingDTO>.Fail(bookingResult.Message ?? "Failed to retrieve booking.");
            }

            var bookingDto = _mapper.Map<BookingDTO>(bookingResult.Data);
            return ServiceResult<BookingDTO>.Ok(bookingDto);
        }
    }
}
