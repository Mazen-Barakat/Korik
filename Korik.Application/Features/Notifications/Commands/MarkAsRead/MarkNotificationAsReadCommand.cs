using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public record MarkNotificationAsReadCommand(MarkNotificationAsReadDto model) : IRequest<ServiceResult<bool>> { }

    public class MarkNotificationAsReadCommandHandler : IRequestHandler<MarkNotificationAsReadCommand, ServiceResult<bool>>
    {
        private readonly INotificationService _notificationService;

     public MarkNotificationAsReadCommandHandler(INotificationService notificationService)
     {
       _notificationService = notificationService;
        }

        public async Task<ServiceResult<bool>> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
        {
  var result = await _notificationService.MarkAsReadAsync(request.model.NotificationId);
     return result;
        }
    }
}
