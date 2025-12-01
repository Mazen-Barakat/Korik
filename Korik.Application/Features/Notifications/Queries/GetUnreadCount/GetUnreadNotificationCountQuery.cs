using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public record GetUnreadNotificationCountQuery(string UserId) : IRequest<ServiceResult<int>> { }

    public class GetUnreadNotificationCountQueryHandler : IRequestHandler<GetUnreadNotificationCountQuery, ServiceResult<int>>
    {
        private readonly INotificationService _notificationService;

        public GetUnreadNotificationCountQueryHandler(INotificationService notificationService)
        {
         _notificationService = notificationService;
        }

        public async Task<ServiceResult<int>> Handle(GetUnreadNotificationCountQuery request, CancellationToken cancellationToken)
        {
            var result = await _notificationService.GetUnreadCountAsync(request.UserId);
      return result;
     }
    }
}
