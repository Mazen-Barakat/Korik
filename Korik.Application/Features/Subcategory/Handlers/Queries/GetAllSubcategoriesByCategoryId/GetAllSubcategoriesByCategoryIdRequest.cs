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
    public record GetAllSubcategoriesByCategoryIdRequest(GetAllSubcategoriesByCategoryIdDTO model) 
        : IRequest<ServiceResult<IEnumerable<SubcategoryDTO>>> { }


    public class GetAllSubcategoriesByCategoryIdRequestHandler
        : IRequestHandler<GetAllSubcategoriesByCategoryIdRequest, ServiceResult<IEnumerable<SubcategoryDTO>>>
    {
        private readonly ISubcategoryService _subcategoryService;
        private readonly IValidator<GetAllSubcategoriesByCategoryIdDTO> _validator;
        private readonly IMapper _mapper;

        public GetAllSubcategoriesByCategoryIdRequestHandler
            (
            ISubcategoryService subcategoryService, 
            IValidator<GetAllSubcategoriesByCategoryIdDTO> validator,
            IMapper mapper
            )
        {
            _subcategoryService = subcategoryService;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<ServiceResult<IEnumerable<SubcategoryDTO>>> Handle(
            GetAllSubcategoriesByCategoryIdRequest request,
            CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.model, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage);
                return ServiceResult<IEnumerable<SubcategoryDTO>>.Fail(string.Join(", ", errors));
            }


            var subcategoriesResult = 
                await _subcategoryService.GetAllSubcategoriesByCategoryIdAsync(request.model.CategoryId);
            if (!subcategoriesResult.Success)
            {
                return ServiceResult<IEnumerable<SubcategoryDTO>>.Fail(subcategoriesResult.Message!);
            }


            var subcategoryDTOs = _mapper.Map<IEnumerable<SubcategoryDTO>>(subcategoriesResult.Data);
            return ServiceResult<IEnumerable<SubcategoryDTO>>.Ok(subcategoryDTOs, "Subcategories retrieved successfully.");

        }
    }

}
