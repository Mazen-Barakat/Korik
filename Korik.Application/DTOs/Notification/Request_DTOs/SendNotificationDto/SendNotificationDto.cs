using FluentValidation;
using Korik.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class SendNotificationDto
    {
        public string SenderId { get; set; } = string.Empty;
      public string ReceiverId { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public NotificationType Type { get; set; }
public int? BookingId { get; set; }
    }

    public class SendNotificationDtoValidator : AbstractValidator<SendNotificationDto>
    {
        public SendNotificationDtoValidator()
        {
       RuleFor(x => x.SenderId)
         .NotEmpty().WithMessage("SenderId is required.");

            RuleFor(x => x.ReceiverId)
      .NotEmpty().WithMessage("ReceiverId is required.");

          RuleFor(x => x.Message)
    .NotEmpty().WithMessage("Message is required.")
     .MaximumLength(500).WithMessage("Message must not exceed 500 characters.");

      RuleFor(x => x.Type)
    .IsInEnum().WithMessage("Invalid notification type.");

       When(x => x.BookingId.HasValue, () =>
            {
          RuleFor(x => x.BookingId.Value)
             .GreaterThan(0).WithMessage("BookingId must be greater than 0.");
      });
        }
    }
}
