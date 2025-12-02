using AutoMapper;
using Korik.Domain;

namespace Korik.Application
{
    public class BookingCarOwnerProfileMapping : Profile
    {
        public BookingCarOwnerProfileMapping()
        {
            CreateMap<CarOwnerProfile, BookingCarOwnerProfileDTO>();
        }
    }
}
