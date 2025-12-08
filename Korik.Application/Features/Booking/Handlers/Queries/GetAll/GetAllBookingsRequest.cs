using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public record GetAllBookingsRequest : IRequest<ServiceResult<IEnumerable<BookingDTO>>> { }


    public class GetAllBookingsRequestHandler : IRequestHandler<GetAllBookingsRequest, ServiceResult<IEnumerable<BookingDTO>>>
    {
        private readonly IBookingService _bookingService;
        private readonly IMapper _mapper;
        public GetAllBookingsRequestHandler(IBookingService bookingService, IMapper mapper)
        {
            _bookingService = bookingService;
            _mapper = mapper;
        }
        public async Task<ServiceResult<IEnumerable<BookingDTO>>> Handle(GetAllBookingsRequest request, CancellationToken cancellationToken)
        {
            var bookingsResult = await _bookingService.GetAllAsync();
            if (!bookingsResult.Success)
            {
                return ServiceResult<IEnumerable<BookingDTO>>.Fail(bookingsResult.Message ?? "Failed to retrieve bookings.");
            }

            var bookingDTOs = _mapper.Map<IEnumerable<BookingDTO>>(bookingsResult.Data);
            return ServiceResult<IEnumerable<BookingDTO>>.Ok(bookingDTOs, "Bookings retrieved successfully.");

        }
    }
}
