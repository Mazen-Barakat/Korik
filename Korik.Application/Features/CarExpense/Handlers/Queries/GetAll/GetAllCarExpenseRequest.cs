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
    public record GetAllCarExpenseRequest : IRequest<ServiceResult<IEnumerable<CarExpenseDTO>>>;

    public class GetAllCarExpenseRequestHandler : IRequestHandler<GetAllCarExpenseRequest, ServiceResult<IEnumerable<CarExpenseDTO>>>
    {
        private readonly ICarExpenseService _service;
        private readonly IMapper _mapper;

        public GetAllCarExpenseRequestHandler(ICarExpenseService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        public async Task<ServiceResult<IEnumerable<CarExpenseDTO>>> Handle(GetAllCarExpenseRequest request, CancellationToken cancellationToken)
        {
            // Retrieve all car expenses
            var expensesResult = await _service.GetAllAsync();

            if (!expensesResult.Success || expensesResult.Data == null)
            {
                return ServiceResult<IEnumerable<CarExpenseDTO>>.Fail("Failed to retrieve car expenses.");
            }

            // Map to DTOs
            var expenseDtos = _mapper.Map<IEnumerable<CarExpenseDTO>>(expensesResult.Data);

            return ServiceResult<IEnumerable<CarExpenseDTO>>.Ok(expenseDtos);
        }
    }
}
