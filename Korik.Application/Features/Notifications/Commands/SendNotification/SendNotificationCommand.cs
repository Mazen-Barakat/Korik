using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
public record SendNotificationCommand(SendNotificationDto model) : IRequest<ServiceResult<NotificationDto>> { }

    public class SendNotificationCommandHandler : IRequestHandler<SendNotificationCommand, ServiceResult<NotificationDto>>
    {
        private readonly INotificationService _notificationService;

        public SendNotificationCommandHandler(INotificationService notificationService)
    {
            _notificationService = notificationService;
        }

        public async Task<ServiceResult<NotificationDto>> Handle(SendNotificationCommand request, CancellationToken cancellationToken)
      {
            var result = await _notificationService.SendNotificationAsync(
           request.model.SenderId,
                request.model.ReceiverId,
       request.model.Message,
           request.model.Type,
     request.model.BookingId
            );

            return result;
        }
    }
}
