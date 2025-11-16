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
    public record DeleteCategoryRequest(DeleteCategoryDTO model) : IRequest<ServiceResult<CategoryDTO>> { }


    public class DeleteCategoryRequestHandler : IRequestHandler<DeleteCategoryRequest, ServiceResult<CategoryDTO>>
    {
        private readonly ICategoryService _categoryService;
        private readonly IValidator<DeleteCategoryDTO> _validator;
        private readonly IMapper _mapper;
        public DeleteCategoryRequestHandler
            (
            ICategoryService categoryService,
            IValidator<DeleteCategoryDTO> validator,
            IMapper mapper
            )
        {
            _categoryService = categoryService;
            _validator = validator;
            _mapper = mapper;
        }
        public async Task<ServiceResult<CategoryDTO>> Handle(DeleteCategoryRequest request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.model, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return ServiceResult<CategoryDTO>.Fail(errors);
            }


            var result = await _categoryService.DeleteAsync(request.model.Id);
            if (!result.Success)
            {
                return ServiceResult<CategoryDTO>.Fail(result.Message ?? "Failed to delete category.");
            }


            var categoryDTO = _mapper.Map<CategoryDTO>(result.Data);
            return ServiceResult<CategoryDTO>.Ok(categoryDTO);
        }
    }
}
