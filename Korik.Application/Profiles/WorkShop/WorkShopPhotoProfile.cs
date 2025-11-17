using AutoMapper;
using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class WorkShopPhotoProfile : Profile
    {
        public WorkShopPhotoProfile()
        {
            CreateMap<CreateWorkShopPhotosDTO, WorkShopPhoto>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PhotoUrl, opt => opt.Ignore())
                .ForMember(dest => dest.WorkShopProfile, opt => opt.Ignore());

            CreateMap<WorkShopPhoto, WorkShopPhotoItemDTO>().ReverseMap();
        }
    }
}