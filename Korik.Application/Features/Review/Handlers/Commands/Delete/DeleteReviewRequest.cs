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
    public record DeleteReviewRequest(DeleteReviewDTO Model) : IRequest<ServiceResult<ReviewDTO>>;

    public class DeleteReviewRequestHandler : IRequestHandler<DeleteReviewRequest, ServiceResult<ReviewDTO>>
    {
        private readonly IReviewService _service;
        private readonly IMapper _mapper;
        private readonly IValidator<DeleteReviewDTO> _validator;

        public DeleteReviewRequestHandler
            (IReviewService service,
            IMapper mapper,
            IValidator<DeleteReviewDTO> validator)
        {
            _service = service;
            _mapper = mapper;
            _validator = validator;
        }
        public async Task<ServiceResult<ReviewDTO>> Handle(DeleteReviewRequest request, CancellationToken cancellationToken)
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
            var deletedReviewResult = await _service.DeleteAsync(request.Model.Id);
            if (!deletedReviewResult.Success)
                return ServiceResult<ReviewDTO>.Fail(deletedReviewResult.Message ?? "Failed to delete review.");
            #endregion      
        
            return ServiceResult<ReviewDTO>.Ok(_mapper.Map<ReviewDTO>(deletedReviewResult.Data));
        }
    }

}
