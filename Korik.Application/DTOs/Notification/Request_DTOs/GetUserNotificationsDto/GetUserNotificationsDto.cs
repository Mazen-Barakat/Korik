using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class GetUserNotificationsDto
    {
  public string UserId { get; set; } = string.Empty;
  }

    public class GetUserNotificationsDtoValidator : AbstractValidator<GetUserNotificationsDto>
    {
   public GetUserNotificationsDtoValidator()
{
  RuleFor(x => x.UserId)
          .NotEmpty().WithMessage("UserId is required.");
        }
    }
}
