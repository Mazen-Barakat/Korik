using AutoMapper;
using FluentValidation;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Korik.Application
{
    public record GetAvgRatingsByWorkShopProfileIdRequest(GetAvgRatingsByWorkShopProfileIdDTO Model) : IRequest<ServiceResult<AvgRatingDTO>>;

    public class GetAvgRatingsByWorkShopProfileIdRequestHandler : IRequestHandler<GetAvgRatingsByWorkShopProfileIdRequest, ServiceResult<AvgRatingDTO>>
    {
        private readonly IReviewService _reviewService;
        private readonly IValidator<GetAvgRatingsByWorkShopProfileIdDTO> _validator;
        private readonly IMapper _mapper;

        public GetAvgRatingsByWorkShopProfileIdRequestHandler(
            IReviewService reviewService,
            IValidator<GetAvgRatingsByWorkShopProfileIdDTO> validator,
            IMapper mapper)
        {
            _reviewService = reviewService;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<ServiceResult<AvgRatingDTO>> Handle(GetAvgRatingsByWorkShopProfileIdRequest request, CancellationToken cancellationToken)
        {
            #region Not Valid

            var validationResult = await _validator.ValidateAsync(request.Model, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                return ServiceResult<AvgRatingDTO>.Fail(errors);
            }

            #endregion Not Valid

            #region Valid

            var averageRatingResult = await _reviewService.GetAverageRatingsByWorkShopProfileIdAsync(request.Model.WorkShopProfileId);
            if (!averageRatingResult.Success)
            {
                return ServiceResult<AvgRatingDTO>.Fail(averageRatingResult.Message ?? "An error occurred while fetching the average rating.");
            }

            #endregion Valid



            return ServiceResult<AvgRatingDTO>.Ok(_mapper.Map<AvgRatingDTO>(averageRatingResult.Data));
        }
    }
}