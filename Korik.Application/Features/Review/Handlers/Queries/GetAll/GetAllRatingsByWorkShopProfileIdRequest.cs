using AutoMapper;
using FluentValidation;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Korik.Application
{
    public record GetAllRatingsByWorkShopProfileIdRequest(GetAllRatingsByWorkShopProfileIdDTO Model) : IRequest<ServiceResult<IEnumerable<ReviewDTO>>>;

    public class GetAllRatingsByWorkShopProfileIdRequestHandler : IRequestHandler<GetAllRatingsByWorkShopProfileIdRequest, ServiceResult<IEnumerable<ReviewDTO>>>
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

        public async Task<ServiceResult<IEnumerable<ReviewDTO>>> Handle(GetAllRatingsByWorkShopProfileIdRequest request, CancellationToken cancellationToken)
        {
            #region Not Valid
            var validationResult = await _validator.ValidateAsync(request.Model, cancellationToken);
            if (!validationResult.IsValid)
            {
                return ServiceResult<IEnumerable<ReviewDTO>>.Fail(validationResult.Errors.First().ErrorMessage);
            }

            #endregion

            #region Valid
            var reviewsResult = await _reviewService.GetAllReviewsByWorkShopProfileIdAsync(request.Model.WorkShopProfileId);
            if (!reviewsResult.Success)
            {
                return ServiceResult<IEnumerable<ReviewDTO>>.Fail(reviewsResult.Message ?? "An error occurred while fetching the reviews.");
            }

            var reviewDTOs = _mapper.Map<IEnumerable<ReviewDTO>>(reviewsResult.Data);

            #endregion            
            
            return ServiceResult<IEnumerable<ReviewDTO>>.Ok(reviewDTOs);
        }
    }
}
