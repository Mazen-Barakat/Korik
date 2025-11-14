using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using AutoMapper;
using MediatR;
using Korik.Domain;

namespace Korik.Application
{
    public record GetAllCarRequest : IRequest<ServiceResult<IEnumerable<CarDTO>>>;

    public class GetAllCarRequestHandler : IRequestHandler<GetAllCarRequest, ServiceResult<IEnumerable<CarDTO>>>
    {
        private readonly ICarService _service;
        private readonly IMapper _mapper;

        public GetAllCarRequestHandler(ICarService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        public async Task<ServiceResult<IEnumerable<CarDTO>>> Handle(GetAllCarRequest request, CancellationToken cancellationToken)
        {
            // Retrieve all cars
            var carsResult = await _service.GetAllAsync();

            if (!carsResult.Success || carsResult.Data == null)
            {
                return ServiceResult<IEnumerable<CarDTO>>.Fail(carsResult.Message?? "Failed to retrieve cars.");
            }

            // Map to DTOs
            var carDtos = _mapper.Map<IEnumerable<CarDTO>>(carsResult.Data);

            return ServiceResult<IEnumerable<CarDTO>>.Ok(carDtos);
        }
    }
}
