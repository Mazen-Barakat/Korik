using AutoMapper;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Korik.Application
{
    public record GetAllCarIndicatorsByCarIdRequest(GetAllIndicatorsByCarIdDTO Model)
        : IRequest<ServiceResult<IEnumerable<CarIndicatorDTO>>>;

    public class GetAllCarIndicatorsByCarIdRequestHandler
        : IRequestHandler<GetAllCarIndicatorsByCarIdRequest, ServiceResult<IEnumerable<CarIndicatorDTO>>>
    {
        private readonly ICarIndicatorService _carIndicatorService;
        private readonly ICarService _carService;
        private readonly ICarIndicatorStatusService _statusService;
        private readonly IValidator<GetAllIndicatorsByCarIdDTO> _validator;
        private readonly IMapper _mapper;

        public GetAllCarIndicatorsByCarIdRequestHandler(
            ICarIndicatorService carIndicatorService,
            ICarService carService,
            ICarIndicatorStatusService statusService,
            IValidator<GetAllIndicatorsByCarIdDTO> validator,
            IMapper mapper)
        {
            _carIndicatorService = carIndicatorService;
            _carService = carService;
            _statusService = statusService;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<ServiceResult<IEnumerable<CarIndicatorDTO>>> Handle(
            GetAllCarIndicatorsByCarIdRequest request, CancellationToken cancellationToken)
        {
            // ------------------------------
            // 1. Validate Request
            // ------------------------------
            var validationResult = await _validator.ValidateAsync(request.Model, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                return ServiceResult<IEnumerable<CarIndicatorDTO>>.Fail(errors);
            }

            // ------------------------------
            // 2. Fetch Car
            // ------------------------------
            var carResult = await _carService.GetByIdAsync(request.Model.CarId);

            if (!carResult.Success || carResult.Data == null)
                return ServiceResult<IEnumerable<CarIndicatorDTO>>.Fail("Car not found.");

            var car = carResult.Data;

            // ------------------------------
            // 3. Fetch Indicators
            // ------------------------------
            var indicatorsResult = await _carIndicatorService.GetAllCarIndicatorsByCarId(request.Model.CarId);

            if (!indicatorsResult.Success)
            {
                return ServiceResult<IEnumerable<CarIndicatorDTO>>.Fail(
                    indicatorsResult.Message ?? "Failed to retrieve car indicators.");
            }

            var indicators = indicatorsResult.Data.ToList();

            // ------------------------------
            // 4. Recalculate dynamic fields
            // ------------------------------
            foreach (var ind in indicators)
            {
                var updated = _statusService.CalculateAll(
                    ind.IndicatorType,
                    ind.LastCheckedDate,
                    ind.NextCheckedDate,
                    ind.NextMileage,
                    car.CurrentMileage
                );

                ind.CarStatus = updated.CarStatus;
                ind.MileageDifference = updated.MileageDifference;
                ind.TimeDifference = updated.TimeDifference;
                ind.TimeDifferenceAsPercentage = updated.TimeDifferenceAsPercentage;
            }

            // ------------------------------
            // 5. Return Mapped DTOs
            // ------------------------------
            return ServiceResult<IEnumerable<CarIndicatorDTO>>
                .Ok(_mapper.Map<IEnumerable<CarIndicatorDTO>>(indicators));
        }
    }
}
