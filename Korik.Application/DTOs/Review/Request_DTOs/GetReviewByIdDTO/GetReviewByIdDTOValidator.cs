using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class GetReviewByIdDTOValidator : AbstractValidator<GetReviewByIdDTO>
    {
        private readonly IReviewRepository _reviewRepository;

        public GetReviewByIdDTOValidator(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;

            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Id must be greater than 0.")
                .MustAsync(async (id, cancellation) => await _reviewRepository.IsExistAsync(id))
                .WithMessage("Review with the given Id does not exist.");
        }
    }
}
