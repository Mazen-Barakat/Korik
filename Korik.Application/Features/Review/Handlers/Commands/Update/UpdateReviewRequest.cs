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
    public record UpdateReviewRequest(UpdateReviewDTO Model) : IRequest<ServiceResult<ReviewDTO>>;

    public class UpdateReviewRequestHandler : IRequestHandler<UpdateReviewRequest, ServiceResult<ReviewDTO>>
    {
        private readonly IReviewService _service;
        private readonly IMapper _mapper;
        private readonly IValidator<UpdateReviewDTO> _validator;
        private readonly IWorkShopProfileService _workShopProfileService;

        public UpdateReviewRequestHandler
            (
            IReviewService service,
            IMapper mapper,
            IValidator<UpdateReviewDTO> validator,
            IWorkShopProfileService workShopProfileService
            )
        {
            _service = service;
            _mapper = mapper;
            _validator = validator;
            _workShopProfileService = workShopProfileService;
        }

        public async Task<ServiceResult<ReviewDTO>> Handle(UpdateReviewRequest request, CancellationToken cancellationToken)
        {
            #region Not Valid

            var validationResult = await _validator.ValidateAsync(request.Model);
            if (!validationResult.IsValid)
            {
                var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                return ServiceResult<ReviewDTO>.Fail(errors);
            }

            #endregion Not Valid

            #region Valid

            var ReviewEntityExistsResult = await _service.GetByIdAsync(request.Model.Id);
            if (!ReviewEntityExistsResult.Success)
            {
                return ServiceResult<ReviewDTO>.Fail(ReviewEntityExistsResult.Message ?? "Review not found.");
            }
            var updatedReviewResult = await _service.UpdateAsync(_mapper.Map<Review>(request.Model));
            if (!updatedReviewResult.Success)
            {
                return ServiceResult<ReviewDTO>.Fail(updatedReviewResult.Message ?? "Failed to update Review.");
            }

            var rating = await _service.GetAverageRatingsByWorkShopProfileIdAsync(updatedReviewResult.Data!.WorkShopProfileId);

            var workShopProfile = await _workShopProfileService.GetByIdAsync(updatedReviewResult.Data!.WorkShopProfileId);

            workShopProfile.Data!.Rating = rating.Data;

            await _workShopProfileService.UpdateAsync(workShopProfile.Data);

            #endregion Valid

            return ServiceResult<ReviewDTO>.Ok(_mapper.Map<ReviewDTO>(updatedReviewResult.Data));
        }
    }
}