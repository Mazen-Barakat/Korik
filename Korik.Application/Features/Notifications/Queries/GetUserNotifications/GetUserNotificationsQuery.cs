using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public record GetUserNotificationsQuery(GetUserNotificationsDto model) : IRequest<ServiceResult<IEnumerable<NotificationDto>>> { }

  public class GetUserNotificationsQueryHandler : IRequestHandler<GetUserNotificationsQuery, ServiceResult<IEnumerable<NotificationDto>>>
    {
   private readonly INotificationService _notificationService;

      public GetUserNotificationsQueryHandler(INotificationService notificationService)
    {
       _notificationService = notificationService;
        }

   public async Task<ServiceResult<IEnumerable<NotificationDto>>> Handle(GetUserNotificationsQuery request, CancellationToken cancellationToken)
        {
            var result = await _notificationService.GetUserNotificationsAsync(request.model.UserId);
  return result;
        }
    }
}
