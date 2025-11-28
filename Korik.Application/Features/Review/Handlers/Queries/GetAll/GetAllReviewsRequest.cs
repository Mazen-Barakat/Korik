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
    public record GetAllReviewsRequest : IRequest<ServiceResult<IEnumerable<ReviewDTO>>>;

    public class GetAllReviewsRequestHandler : IRequestHandler<GetAllReviewsRequest, ServiceResult<IEnumerable<ReviewDTO>>>
    {
        private readonly IReviewService _service;
        private readonly IMapper _mapper;

        public GetAllReviewsRequestHandler(
            IReviewService service,
            IMapper mapper,
            IValidator<GetAllReviewsDTO> validator)
        {
            _service = service;
            _mapper = mapper;
        }

        public async Task<ServiceResult<IEnumerable<ReviewDTO>>> Handle(GetAllReviewsRequest request, CancellationToken cancellationToken)
        {
            var reviewsResult = await _service.GetAllAsync();

            if (!reviewsResult.Success || reviewsResult.Data == null)
            {
                return ServiceResult<IEnumerable<ReviewDTO>>.Fail(reviewsResult.Message ?? "Failed to retrieve reviews.");
            }

            var reviewDTOs = _mapper.Map<IEnumerable<ReviewDTO>>(reviewsResult.Data);

            return ServiceResult<IEnumerable<ReviewDTO>>.Ok(reviewDTOs);
        }
    }
}









