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
        private readonly IReviewService _reviewService;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateReviewDTO> _validator;
        private readonly IWorkShopProfileService _workShopProfileService;

        public CreateReviewRequestHandler
            (
            IReviewService reviewService,
            IMapper mapper,
            IValidator<CreateReviewDTO> validator,
            IWorkShopProfileService workShopProfileService
            )
        {
            _reviewService = reviewService;
            _mapper = mapper;
            _validator = validator;
            _workShopProfileService = workShopProfileService;
        }

        public async Task<ServiceResult<ReviewDTO>> Handle(CreateReviewRequest request, CancellationToken cancellationToken)
        {
            #region Not Valid

            var validationResult = await _validator.ValidateAsync(request.Model);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return ServiceResult<ReviewDTO>.Fail(errors);
            }

            #endregion Not Valid

            #region Valid

            var createdReviewResult = await _reviewService.CreateAsync(entity: _mapper.Map<Review>(request.Model));

            if (!createdReviewResult.Success)
                return ServiceResult<ReviewDTO>.Fail(createdReviewResult.Message ?? "Failed to create review.");

            var rating = await _reviewService.GetAverageRatingsByWorkShopProfileIdAsync(createdReviewResult.Data!.WorkShopProfileId);

            var workShopProfile = await _workShopProfileService.GetByIdAsync(createdReviewResult.Data!.WorkShopProfileId);

            workShopProfile.Data!.Rating = rating.Data;

            await _workShopProfileService.UpdateAsync(workShopProfile.Data);

            #endregion Valid

            return ServiceResult<ReviewDTO>.Created(_mapper.Map<ReviewDTO>(createdReviewResult.Data));
        }
    }
}