using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public record SendEmailRequest(EmailDTO model) : IRequest<ServiceResult<string>> { }


    public class SendEmailRequestHandler : IRequestHandler<SendEmailRequest, ServiceResult<string>>
    {
        private readonly IAccountService _accountService;

        public SendEmailRequestHandler(IAccountService accountService)
        {
            _accountService = accountService;
        }
        public async Task<ServiceResult<string>> Handle(SendEmailRequest request, CancellationToken cancellationToken)
        {
            var result = await _accountService.SendEmailAsync(request.model);

            return result;
        }
    }
}

