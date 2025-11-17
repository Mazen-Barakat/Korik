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
    public record DeleteSubcategoryRequest(DeleteSubcategoryDTO model) : IRequest<ServiceResult<SubcategoryDTO>>{ }


    public class DeleteSubcategoryRequestHandler : IRequestHandler<DeleteSubcategoryRequest, ServiceResult<SubcategoryDTO>>
    {
        private readonly ISubcategoryService _subcategoryService;
        private readonly IValidator<DeleteSubcategoryDTO> _validator;
        private readonly IMapper _mapper;
        public DeleteSubcategoryRequestHandler
            (
            ISubcategoryService subcategoryService,
            IValidator<DeleteSubcategoryDTO> validator,
            IMapper mapper
            )
        {
            _subcategoryService = subcategoryService;
            _validator = validator;
            _mapper = mapper;
        }


        public async Task<ServiceResult<SubcategoryDTO>> Handle(DeleteSubcategoryRequest request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.model, cancellationToken);
            if (!validationResult.IsValid)
            {
            var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            return ServiceResult<SubcategoryDTO>.Fail(errors);
            }


            var deletedSubcategoryResult = await _subcategoryService.DeleteAsync(request.model.Id);
            if (!deletedSubcategoryResult.Success)
            {
                return ServiceResult<SubcategoryDTO>.Fail(deletedSubcategoryResult.Message ?? "Failed to delete subcategory.");
            }


            var subcategoryDto = _mapper.Map<SubcategoryDTO>(deletedSubcategoryResult.Data);
            return ServiceResult<SubcategoryDTO>.Ok(subcategoryDto, "Subcategory deleted successfully.");
        }
    }
}
