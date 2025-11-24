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
    public record CreateSubcategoryRequest(CreateSubcategoryDTO model) : IRequest<ServiceResult<SubcategoryDTO>> { }



    public class CreateSubcategoryRequestHandler : IRequestHandler<CreateSubcategoryRequest, ServiceResult<SubcategoryDTO>>
    {
        private readonly ISubcategoryService _subcategoryService;
        private readonly IValidator<CreateSubcategoryDTO> _validator;
        private readonly IMapper _mapper;
        public CreateSubcategoryRequestHandler
            (
            ISubcategoryService subcategoryService,
            IValidator<CreateSubcategoryDTO> validator,
            IMapper mpper
            )
        {
            _subcategoryService = subcategoryService;
            _validator = validator;
            _mapper = mpper;
        }

        public async Task<ServiceResult<SubcategoryDTO>> Handle(CreateSubcategoryRequest request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.model, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return ServiceResult<SubcategoryDTO>.Fail(string.Join(", ", errors));
            }

            var subcategory= _mapper.Map<Subcategory>(request.model);
            var createdSubcategoryResult = await _subcategoryService.CreateAsync(subcategory);

            if (!createdSubcategoryResult.Success)
            {
                return ServiceResult<SubcategoryDTO>.Fail(createdSubcategoryResult.Message ?? "Failed to create subcategory.");
            }

            var subcategoryDTO = _mapper.Map<SubcategoryDTO>(createdSubcategoryResult.Data);
            return ServiceResult<SubcategoryDTO>.Created(subcategoryDTO, "Subcategory created successfully.");


        }
    }
}
