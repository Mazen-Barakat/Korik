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
    public record GetByIdReviewRequest(GetReviewByIdDTO Model) : IRequest<ServiceResult<ReviewDTO>>;

    public class GetByIdReviewRequestHandler : IRequestHandler<GetByIdReviewRequest, ServiceResult<ReviewDTO>>
    {
        private readonly IReviewService _service;
        private readonly IMapper _mapper;
        private readonly IValidator<GetReviewByIdDTO> _validator;

        public GetByIdReviewRequestHandler
            (IReviewService service, 
            IMapper mapper, 
            IValidator<GetReviewByIdDTO> validator)
        {
            _service = service;
            _mapper = mapper;
            _validator = validator;
        }


        public async Task<ServiceResult<ReviewDTO>> Handle(GetByIdReviewRequest request, CancellationToken cancellationToken)
        {
            #region Not Valid
            var validationResult = await _validator.ValidateAsync(request.Model, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return ServiceResult<ReviewDTO>.Fail($"Validation failed: {errors}");
            }
            #endregion        

            var reviewResult = await _service.GetByIdAsync(request.Model.Id);

            if(!reviewResult.Success)
            {
                return ServiceResult<ReviewDTO>.Fail(reviewResult.Message ?? "Review not found");
            }

            return ServiceResult<ReviewDTO>.Ok(_mapper.Map<ReviewDTO>(reviewResult.Data));
        }
    }
}