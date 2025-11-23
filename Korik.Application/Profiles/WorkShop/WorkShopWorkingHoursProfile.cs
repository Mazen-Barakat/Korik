using AutoMapper;
using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class WorkShopWorkingHoursProfile : Profile
    {
        public WorkShopWorkingHoursProfile()
        {
            CreateMap<WorkingHours, WorkShopWorkingHoursDTO>().ReverseMap();
            CreateMap<CreateWorkShopWorkingHoursDTO, WorkingHours>();
            CreateMap<UpdateWorkShopWorkingHoursDTO, WorkingHours>();
        }
    }
}