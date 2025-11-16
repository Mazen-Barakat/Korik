using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Korik.Domain;

namespace Korik.Application
{
    public class CarIndicatorProfile : Profile
    {
        public CarIndicatorProfile()
        {
            // Map CreateCarIndicatorDTO to CarIndicator
            CreateMap<CreateCarIndicatorDTO, CarIndicator>().ReverseMap();

            // Map CarIndicator to CarIndicatorDTO
            CreateMap<CarIndicator, CarIndicatorDTO>().ReverseMap();

            // Map UpdateCarIndicatorDTO to CarIndicator
            CreateMap<UpdateCarIndicatorDTO, CarIndicator>().ReverseMap();
        }
    }
}
