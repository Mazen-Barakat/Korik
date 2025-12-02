using AutoMapper;
using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class CarOwnerProfileMapping : Profile
    {
        public CarOwnerProfileMapping()
        {
            //Entity => DTO || DTO => Entity
            CreateMap<CarOwnerProfile, CarOwnerProfileDTO>().ReverseMap();

            //Request => CarOwnerProfile || CarOwnerProfile => Request
            CreateMap<CarOwnerProfile, CreateCarOwnerProfileDTO>().ReverseMap();

            CreateMap<CarOwnerProfile, UpdateCarOwnerProfileDTO>().ReverseMap()
                            .ForAllMembers(opt =>
                                opt.Condition((src, dest, srcMember) => srcMember != null));
            // Map CarOwnerProfile to CarOwnerProfileWithCarDto
            CreateMap<CarOwnerProfile, CarOwnerProfileWithCarDTO>();

            // Map Car to ProfileWithCarDto
            CreateMap<Car, ProfileWithCarDTO>();

            // Map CarOwnerProfile to BookingCarOwnerProfileDTO (for booking-related queries)
            CreateMap<CarOwnerProfile, BookingCarOwnerProfileDTO>();
        }
    }
}