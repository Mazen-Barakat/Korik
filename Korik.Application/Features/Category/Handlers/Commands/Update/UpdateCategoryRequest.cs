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
    public record UpdateCategoryRequest(UpdateCategoryDTO model) : IRequest<ServiceResult<CategoryDTO>> { }


    public class UpdateCategoryRequestHandler : IRequestHandler<UpdateCategoryRequest, ServiceResult<CategoryDTO>>
    {
        private readonly ICategoryService _categoryService;
        private readonly IValidator<UpdateCategoryDTO> _validator;
        private readonly IMapper _mapper;
        public UpdateCategoryRequestHandler
            (
            ICategoryService categoryService,
            IValidator<UpdateCategoryDTO> validator,
            IMapper mapper
            )
        {
            _categoryService = categoryService;
            _validator = validator;
            _mapper = mapper;
        }
        public async Task<ServiceResult<CategoryDTO>> Handle(UpdateCategoryRequest request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.model, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return ServiceResult<CategoryDTO>.Fail(errors);
            }

            var category = _mapper.Map<Category>(request.model);
            var updatedCategoryResult = await _categoryService.UpdateAsync(category);


            if (!updatedCategoryResult.Success)
            {
                return ServiceResult<CategoryDTO>.Fail(updatedCategoryResult.Message ?? "Failed to update category.");
            }


            var catergoryDTO = _mapper.Map<CategoryDTO>(updatedCategoryResult.Data);
            return ServiceResult<CategoryDTO>.Ok(catergoryDTO);

        }
    }
}
