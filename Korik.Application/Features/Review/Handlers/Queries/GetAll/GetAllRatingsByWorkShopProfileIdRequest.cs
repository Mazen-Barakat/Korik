using AutoMapper;
using FluentValidation;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Korik.Application
{
    public record GetAllRatingsByWorkShopProfileIdRequest(GetAllRatingsByWorkShopProfileIdDTO Model) : IRequest<ServiceResult<IEnumerable<ReviewWithProfileDTO>>>;

    public class GetAllRatingsByWorkShopProfileIdRequestHandler : IRequestHandler<GetAllRatingsByWorkShopProfileIdRequest, ServiceResult<IEnumerable<ReviewWithProfileDTO>>>
    {
        private readonly IReviewService _reviewService;
        private readonly IValidator<GetAllRatingsByWorkShopProfileIdDTO> _validator;
        private readonly IMapper _mapper;

        public GetAllRatingsByWorkShopProfileIdRequestHandler(
            IReviewService reviewService,
            IValidator<GetAllRatingsByWorkShopProfileIdDTO> validator,
            IMapper mapper)
        {
            _reviewService = reviewService;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<ServiceResult<IEnumerable<ReviewWithProfileDTO>>> Handle(GetAllRatingsByWorkShopProfileIdRequest request, CancellationToken cancellationToken)
        {
            #region Not Valid

            var validationResult = await _validator.ValidateAsync(request.Model, cancellationToken);
            if (!validationResult.IsValid)
            {
                return ServiceResult<IEnumerable<ReviewWithProfileDTO>>.Fail(validationResult.Errors.First().ErrorMessage);
            }

            #endregion Not Valid

            #region Valid

            var reviewsResult = await _reviewService.GetAllReviewsByWorkShopProfileIdAsync(request.Model.WorkShopProfileId);
            if (!reviewsResult.Success)
            {
                return ServiceResult<IEnumerable<ReviewWithProfileDTO>>.Fail(reviewsResult.Message ?? "An error occurred while fetching the reviews.");
            }

            var reviewDTOs = _mapper.Map<IEnumerable<ReviewWithProfileDTO>>(reviewsResult.Data);

            #endregion Valid

            return ServiceResult<IEnumerable<ReviewWithProfileDTO>>.Ok(reviewDTOs);
        }
    }
}