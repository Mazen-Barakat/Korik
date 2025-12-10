using AutoMapper;
using Korik.Domain;

namespace Korik.Application
{
    public class PaymentProfile : Profile
    {
        public PaymentProfile()
        {
            // Payment entity to PaymentDTO
            CreateMap<Payment, PaymentDTO>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.StripePaymentStatus));

            // Payment entity to PendingPayoutDTO
            CreateMap<Payment, PendingPayoutDTO>()
                .ForMember(dest => dest.PaymentId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.DaysPending, opt => opt.MapFrom(src => 
                    src.PaidAt.HasValue ? (DateTime.UtcNow - src.PaidAt.Value).Days : 0))
                .ForMember(dest => dest.WorkshopId, opt => opt.MapFrom(src => src.Booking.WorkShopProfileId))
                .ForMember(dest => dest.WorkshopName, opt => opt.MapFrom(src => src.Booking.WorkShopProfile.Name))
                .ForMember(dest => dest.WorkshopPhone, opt => opt.MapFrom(src => src.Booking.WorkShopProfile.PhoneNumber))
                .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.Booking.WorkshopService.Service.Name))
                .ForMember(dest => dest.CarOwnerName, opt => opt.MapFrom(src => 
                    src.Booking.Car.CarOwnerProfile.ApplicationUser.UserName ?? "Unknown"))
                .ForMember(dest => dest.AppointmentDate, opt => opt.MapFrom(src => src.Booking.AppointmentDate))
                .ForMember(dest => dest.CompletedDate, opt => opt.MapFrom(src => src.Booking.CreatedAt))
                .ForMember(dest => dest.PaidAt, opt => opt.MapFrom(src => src.PaidAt ?? DateTime.UtcNow));
        }
    }
}
