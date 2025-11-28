using AutoMapper;
using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class BookingPhotoProfile : Profile
    {
        public BookingPhotoProfile()
        {
            CreateMap<BookingPhoto, BookingPhotoItemDTO>().ReverseMap();

            CreateMap<CreateBookingPhotoDTO, BookingPhoto>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PhotoUrl, opt => opt.Ignore())
                .ForMember(dest => dest.BookingId, opt => opt.Ignore());
        }
    }
}