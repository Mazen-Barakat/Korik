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
    public record GetAllBookingPhotoByBookingIdRequest(GetAllBookingPhotoByBookingIdDTO Model)
        : IRequest<ServiceResult<IEnumerable<BookingPhotoItemDTO>>>
    {
    }

    public class GetAllBookingPhotoByBookingIdRequestHandler
        : IRequestHandler<GetAllBookingPhotoByBookingIdRequest, ServiceResult<IEnumerable<BookingPhotoItemDTO>>>
    {
        #region Dependency Injection

        private readonly IBookingPhotoService _bookingPhotoService;
        private readonly IValidator<GetAllBookingPhotoByBookingIdDTO> _validator;
        private readonly IMapper _mapper;

        public GetAllBookingPhotoByBookingIdRequestHandler
            (
            IBookingPhotoService bookingPhotoService,
            IValidator<GetAllBookingPhotoByBookingIdDTO> validator,
            IMapper mapper
            )
        {
            _bookingPhotoService = bookingPhotoService;
            _validator = validator;
            _mapper = mapper;
        }

        #endregion Dependency Injection

        public async Task<ServiceResult<IEnumerable<BookingPhotoItemDTO>>> Handle(GetAllBookingPhotoByBookingIdRequest request, CancellationToken cancellationToken)
        {
            #region Not Valid

            var validationResult = await _validator.ValidateAsync(request.Model, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return ServiceResult<IEnumerable<BookingPhotoItemDTO>>.Fail(errors);
            }

            #endregion Not Valid

            #region Valid

            var result = await _bookingPhotoService.GetAllPhotosByBookingIdAsync(request.Model.BookingId);

            //Not Valid
            if (!result.Success)
            {
                return ServiceResult<IEnumerable<BookingPhotoItemDTO>>.Fail($"Bad Request - Error While getting Data: {result.Message}");
            }

            //Valid
            //Map Entity(IEnumerable) => DTO (IEnumerable)
            var bookingPhotoItemDTO = _mapper.Map<IEnumerable<BookingPhotoItemDTO>>(result.Data);

            return ServiceResult<IEnumerable<BookingPhotoItemDTO>>.Ok(bookingPhotoItemDTO);

            #endregion Valid
        }
    }
}