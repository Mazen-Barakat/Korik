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
    public record GetAllCarExpensesByCarIdRequest(GetAllCarExpensesByCarIdDTO Model) : IRequest<ServiceResult<IEnumerable<CarExpenseDTO>>>;

    public class GetAllCarExpensesByCarIdRequestHandler : IRequestHandler<GetAllCarExpensesByCarIdRequest, ServiceResult<IEnumerable<CarExpenseDTO>>>
    {
        private readonly ICarExpenseService _carExpenseService;
        private readonly IValidator<GetAllCarExpensesByCarIdDTO> _validator;
        private readonly IMapper _mapper;

        public GetAllCarExpensesByCarIdRequestHandler(
            ICarExpenseService carExpenseService,
            IValidator<GetAllCarExpensesByCarIdDTO> validator,
            IMapper mapper)
        {
            _carExpenseService = carExpenseService;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<ServiceResult<IEnumerable<CarExpenseDTO>>> Handle(GetAllCarExpensesByCarIdRequest request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.Model, cancellationToken);
            #region Not Valid
            if (!validationResult.IsValid)
            {
                var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                return ServiceResult<IEnumerable<CarExpenseDTO>>.Fail(errors);
            }
            #endregion

            #region Valid
            var carExpensesResult = await _carExpenseService.GetAllCarExpensesByCarId(request.Model.CarId);

            var carExpenseDTOs = _mapper.Map<IEnumerable<CarExpenseDTO>>(carExpensesResult.Data);
            #endregion

            return ServiceResult<IEnumerable<CarExpenseDTO>>.Ok(carExpenseDTOs);
        }
    }
}
