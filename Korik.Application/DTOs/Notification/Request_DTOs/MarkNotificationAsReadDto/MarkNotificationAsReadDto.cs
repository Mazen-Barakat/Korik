using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class MarkNotificationAsReadDto
    {
   public int NotificationId { get; set; }
    }

    public class MarkNotificationAsReadDtoValidator : AbstractValidator<MarkNotificationAsReadDto>
 {
        public MarkNotificationAsReadDtoValidator()
        {
  RuleFor(x => x.NotificationId)
   .GreaterThan(0).WithMessage("NotificationId must be greater than 0.");
        }
    }
}
