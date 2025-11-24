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
    public record CreateCategoryRequest(CreateCategoryDTO model) : IRequest<ServiceResult<CategoryDTO>> { }
    

    public class CreateCategoryRequestHandler : IRequestHandler<CreateCategoryRequest, ServiceResult<CategoryDTO>>
    {
        private readonly ICategoryService _categoryService;
        private readonly IValidator<CreateCategoryDTO> _validator;
        private readonly IMapper _mapper;
        public CreateCategoryRequestHandler
            (
            ICategoryService categoryService,
            IValidator<CreateCategoryDTO> validator,
            IMapper mapper

            )
        {
            _categoryService = categoryService;
            _validator = validator;
            _mapper = mapper;
        }
        public async Task<ServiceResult<CategoryDTO>> Handle(CreateCategoryRequest request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.model, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return ServiceResult<CategoryDTO>.Fail(errors);
            }


            var category = _mapper.Map<Category>(request.model);

            var createdCategoryResult = await _categoryService.CreateAsync(category);
            
            if (!createdCategoryResult.Success)
            {
                return ServiceResult<CategoryDTO>.Fail(createdCategoryResult.Message ?? "Failed to create Category.");
            }



            var categoryDTO = _mapper.Map<CategoryDTO>(createdCategoryResult.Data);

            return ServiceResult<CategoryDTO>.Created(categoryDTO);
        }
    }
}
