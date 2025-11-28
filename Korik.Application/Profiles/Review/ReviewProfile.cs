using AutoMapper;
using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class ReviewProfile : Profile
    {
        public ReviewProfile()
        {
            CreateMap<CreateReviewDTO, Review>().ReverseMap();
            CreateMap<Review, ReviewDTO>().ReverseMap();
            CreateMap<UpdateReviewDTO, Review>().ReverseMap();

            // Mapping for AvgRatingDTO
            CreateMap<double, AvgRatingDTO>()
                .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src => src));
        }
    }
}
