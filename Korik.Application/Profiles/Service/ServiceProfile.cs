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
            CreateMap<Service, ServiceDTO>()
                .ForMember(dest => dest.SubcategoryName,
                    opt => opt.MapFrom(src => src.Subcategory != null ? src.Subcategory.Name : null))
                .ReverseMap();

            CreateMap<CreateServiceDTO, Service>();
            CreateMap<UpdateServiceDTO, Service>();
        }
    }
}