using AutoMapper;
using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class NotificationProfile : Profile
    {
        public NotificationProfile()
      {
            CreateMap<Notification, NotificationDto>();
            CreateMap<SendNotificationDto, Notification>()
 .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.IsRead, opt => opt.MapFrom(src => false));
}
    }
}
