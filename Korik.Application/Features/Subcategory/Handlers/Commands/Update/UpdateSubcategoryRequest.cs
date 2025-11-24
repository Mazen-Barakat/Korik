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
    public record UpdateSubcategoryRequest(UpdateSubcategoryDTO model) : IRequest<ServiceResult<SubcategoryDTO>> { }


    public class UpdateSubcategoryRequestHandler : IRequestHandler<UpdateSubcategoryRequest, ServiceResult<SubcategoryDTO>>
    {
        private readonly ISubcategoryService _subcategoryService;
        private readonly IValidator<UpdateSubcategoryDTO> _validator;
        private readonly IMapper _mapper;

        public UpdateSubcategoryRequestHandler
            (
            ISubcategoryService subcategoryService,
            IValidator<UpdateSubcategoryDTO> validator,
            IMapper mapper
            )
        {
            _subcategoryService = subcategoryService;
            _validator = validator;
            _mapper = mapper;
        }


        public async Task<ServiceResult<SubcategoryDTO>> Handle(UpdateSubcategoryRequest request, CancellationToken cancellationToken)
        {
           var validationResult = await _validator.ValidateAsync(request.model, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return ServiceResult<SubcategoryDTO>.Fail(string.Join(", ", errors));
            }

            var updatingSubcategory = _mapper.Map<Subcategory>(request.model);

            var updatedSubcategoryResult = await _subcategoryService.UpdateAsync(updatingSubcategory);

            if (!updatedSubcategoryResult.Success)
            {
                return ServiceResult<SubcategoryDTO>.Fail(updatedSubcategoryResult.Message ?? "Failed to update subcategory.");
            }

            var subcategoryDTO = _mapper.Map<SubcategoryDTO>(updatedSubcategoryResult.Data);
            return ServiceResult<SubcategoryDTO>.Ok(subcategoryDTO, "Subcategory updated successfully.");
        }
    }
}
