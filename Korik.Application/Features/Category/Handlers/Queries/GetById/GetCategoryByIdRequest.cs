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
    public record GetCategoryByIdRequest(GetCategoryByIdDTO model) : IRequest<ServiceResult<CategoryDTO>> { }


    public class GetCategoryByIdRequestHandler : IRequestHandler<GetCategoryByIdRequest, ServiceResult<CategoryDTO>>
    {
        private readonly ICategoryService _categoryService;
        private readonly IValidator<GetCategoryByIdDTO> _validator;
        private readonly IMapper _mapper;
        public GetCategoryByIdRequestHandler(
            ICategoryService categoryService,
            IValidator<GetCategoryByIdDTO> validator,
            IMapper mapper
            )
        {
            _categoryService = categoryService;
            _mapper = mapper;
            _validator = validator;
        }
        public async Task<ServiceResult<CategoryDTO>> Handle(GetCategoryByIdRequest request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.model, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return ServiceResult<CategoryDTO>.Fail(errors);
            }


            var categoryresult = await _categoryService.GetByIdAsync(request.model.Id);

            if (!categoryresult.Success)
            {
                return ServiceResult<CategoryDTO>.Fail(categoryresult.Message ?? $"Failed to Fetch Category with Id = {request.model.Id}");
            }


            var categoryDto = _mapper.Map<CategoryDTO>(categoryresult.Data);
            return ServiceResult<CategoryDTO>.Ok(categoryDto);
        }
    }
}
