using AutoMapper;
using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class ServiceProfile : Profile
    {
        public ServiceProfile()
        {
            CreateMap<Service , CreateServiceDTO>().ReverseMap();

            CreateMap<Service , ServiceDTO>().ReverseMap();

        }
    }
}
