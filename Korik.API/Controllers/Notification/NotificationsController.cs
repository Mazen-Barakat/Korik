using Korik.Application;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace Korik.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
  public class NotificationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public NotificationsController(IMediator mediator)
  {
 _mediator = mediator;
        }

    /// <summary>
      /// Get all notifications for the current user
        /// </summary>
        [HttpGet]
[SwaggerOperation(
     Summary = "Get user notifications",
    Description = "Retrieves all notifications for the authenticated user, ordered by creation date (newest first)."
        )]
        public async Task<IActionResult> GetUserNotifications()
        {
          var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

 if (string.IsNullOrEmpty(userId))
   {
         return ApiResponse.FromResult(this, ServiceResult<IEnumerable<NotificationDto>>.Fail("User not authenticated."));
            }

          var result = await _mediator.Send(new GetUserNotificationsQuery(new GetUserNotificationsDto { UserId = userId }));

    return ApiResponse.FromResult(this, result);
      }

        /// <summary>
        /// Get unread notification count for the current user
        /// </summary>
        [HttpGet("unread-count")]
        [SwaggerOperation(
 Summary = "Get unread notification count",
         Description = "Returns the number of unread notifications for the authenticated user."
        )]
  public async Task<IActionResult> GetUnreadCount()
        {
   var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
          {
              return ApiResponse.FromResult(this, ServiceResult<int>.Fail("User not authenticated."));
       }

            var result = await _mediator.Send(new GetUnreadNotificationCountQuery(userId));

     return ApiResponse.FromResult(this, result);
        }

   /// <summary>
        /// Mark a notification as read
        /// </summary>
        [HttpPut("{id}/mark-read")]
        [SwaggerOperation(
       Summary = "Mark notification as read",
            Description = "Marks a specific notification as read by its ID."
        )]
  public async Task<IActionResult> MarkAsRead([FromRoute] int id)
        {
            var result = await _mediator.Send(new MarkNotificationAsReadCommand(new MarkNotificationAsReadDto { NotificationId = id }));

        return ApiResponse.FromResult(this, result);
        }
    }
}
