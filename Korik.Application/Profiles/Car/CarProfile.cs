using AutoMapper;
using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class CarProfile : Profile
    {
        public CarProfile()
        {
            CreateMap<Car, CarDTO>().ReverseMap();
            CreateMap<CreateCarDTO, Car>().ReverseMap();
            CreateMap<UpdateCarDTO, Car>().ReverseMap();
            CreateMap<GetCarsByCarOwnerProfileIdDTO, Car>().ReverseMap();
        }
    }
}
