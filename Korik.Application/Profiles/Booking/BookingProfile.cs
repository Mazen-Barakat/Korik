using AutoMapper;
using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class BookingProfile : Profile
    {
        public BookingProfile()
        {
            CreateMap<CreateBookingDTO, Booking>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => BookingStatus.Pending))
                .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => src.PaidAmount.HasValue && src.PaidAmount.Value > 0 ? PaymentStatus.Paid : PaymentStatus.Unpaid))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<Booking, BookingDTO>();

            CreateMap<UpdateBookingDTO, Booking>();
        }
    }
}
