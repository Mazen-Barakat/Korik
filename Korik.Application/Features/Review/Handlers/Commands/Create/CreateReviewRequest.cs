using AutoMapper;
using FluentValidation;
using Korik.Domain;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public record CreateReviewRequest(CreateReviewDTO Model) : IRequest<ServiceResult<ReviewDTO>>;

    public class CreateReviewRequestHandler : IRequestHandler<CreateReviewRequest, ServiceResult<ReviewDTO>>
    {
        private readonly IReviewService _service;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateReviewDTO> _validator;

        public CreateReviewRequestHandler
            (IReviewService service,
            IMapper mapper,
            IValidator<CreateReviewDTO> validator)
        {
            _service = service;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<ServiceResult<ReviewDTO>> Handle(CreateReviewRequest request, CancellationToken cancellationToken)
        {
            #region Not Valid
            var validationResult = await _validator.ValidateAsync(request.Model);
            if (!validationResult.IsValid)
            {
                var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                return ServiceResult<ReviewDTO>.Fail(errors);
            }

            #endregion

            #region Valid
            var createdReviewResult = await _service.CreateAsync(_mapper.Map<Review>(request.Model));

            if (!createdReviewResult.Success)
                return ServiceResult<ReviewDTO>.Fail(createdReviewResult.Message ?? "Failed to create review.");
            #endregion        

            return ServiceResult<ReviewDTO>.Created(_mapper.Map<ReviewDTO>(createdReviewResult.Data));
        }
    }
}