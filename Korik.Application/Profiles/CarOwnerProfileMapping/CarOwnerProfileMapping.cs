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
        }
    }
}