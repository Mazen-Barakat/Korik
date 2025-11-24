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
    public record GetSubcategoryByIdRequest(GetSubcategoryByIdDTO model) : IRequest<ServiceResult<SubcategoryDTO>> { }


    public class GetSubcategoryByIdRequestHandler : IRequestHandler<GetSubcategoryByIdRequest, ServiceResult<SubcategoryDTO>>
    {
        private readonly ISubcategoryService _subcategoryService;
        private readonly IValidator<GetSubcategoryByIdDTO> _validator;
        private readonly IMapper _mapper;
        public GetSubcategoryByIdRequestHandler
            (
            ISubcategoryService subcategoryService,
            IValidator<GetSubcategoryByIdDTO> validator,
            IMapper mapper
            )
        {
            _subcategoryService = subcategoryService;
            _validator = validator;
            _mapper = mapper;
        }
        public async Task<ServiceResult<SubcategoryDTO>> Handle(GetSubcategoryByIdRequest request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.model, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return ServiceResult<SubcategoryDTO>.Fail(string.Join("; ", errors));
            }

            var result = await _subcategoryService.GetByIdAsync(request.model.Id);
            if (!result.Success)
            {
                return ServiceResult<SubcategoryDTO>.Fail(result.Message ?? "Subcategory not found.");
            }

            var subcategoryDto = _mapper.Map<SubcategoryDTO>(result.Data);
            return ServiceResult<SubcategoryDTO>.Ok(subcategoryDto);

        }
    }
}
