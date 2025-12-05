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

            CreateMap<Review, ReviewWithProfileDTO>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.CarOwnerProfile.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.CarOwnerProfile.LastName))
                .ForMember(dest => dest.ProfileImageUrl, opt => opt.MapFrom(src => src.CarOwnerProfile.ProfileImageUrl));
        }
    }
}