using AutoMapper;
using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    internal class WorkShopProfileMapping : Profile
    {
        public WorkShopProfileMapping()
        {
            //Entity => DTO || DTO => Entity
            CreateMap<WorkShopProfile, WorkShopProfileDTO>().ReverseMap();

            //Request => WorkShopProfile || WorkShopProfile => Request
            CreateMap<WorkShopProfile, UpdateWorkShopProfileDTO>().ReverseMap()
                .ForAllMembers(opt =>
                                opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<WorkShopProfile, UpdateWorkShopProfileStatusDTO>().ReverseMap()
                .ForAllMembers(opt =>
                                opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<PagedResult<WorkShopProfile>, PagedResult<WorkShopProfileDTO>>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));
        }
    }
}