using AutoMapper;
using FluentValidation;
using MediatR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public record GetBookingServicesWithReviewByCarIdRequest(GetBookingServicesWithReviewByCarIdDTO Model) :
        IRequest<ServiceResult<IEnumerable<BookingServicesWithReviewDTO>>>
    {
    }

    public class GetBookingServicesWithReviewByCarIdRequestHandler :
        IRequestHandler<GetBookingServicesWithReviewByCarIdRequest, ServiceResult<IEnumerable<BookingServicesWithReviewDTO>>>
    {
        #region Dependency Injection

        private readonly IBookingService _bookingService;
        private readonly IValidator<GetBookingServicesWithReviewByCarIdDTO> _validator;

        public GetBookingServicesWithReviewByCarIdRequestHandler
            (
            IBookingService bookingService,
            IValidator<GetBookingServicesWithReviewByCarIdDTO> validator
            )
        {
            _bookingService = bookingService;
            _validator = validator;
        }

        #endregion Dependency Injection

        public async Task<ServiceResult<IEnumerable<BookingServicesWithReviewDTO>>> Handle(GetBookingServicesWithReviewByCarIdRequest request, CancellationToken cancellationToken)
        {
            #region Not Valid

            var validationResult = await _validator.ValidateAsync(request.Model, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(',', validationResult.Errors.Select(e => e.ErrorMessage));
                return ServiceResult<IEnumerable<BookingServicesWithReviewDTO>>.Fail(errors);
            }

            #endregion Not Valid

            #region Valid

            var result = await _bookingService.GetBookingServicesWithReviewAsync(request.Model.CarId);

            //Not Valid
            if (!result.Success)
            {
                return ServiceResult<IEnumerable<BookingServicesWithReviewDTO>>.Fail($"Bad Request - Error While getting Data: {result.Message}");
            }

            //valid

            return ServiceResult<IEnumerable<BookingServicesWithReviewDTO>>.Ok(result.Data!);

            #endregion Valid
        }
    }
}