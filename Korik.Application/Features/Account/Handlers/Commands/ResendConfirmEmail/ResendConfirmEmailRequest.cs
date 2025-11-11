using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public record ResendConfirmEmailRequest(ResendConfirmationEmailDTO model, string origin) : IRequest<ServiceResult<string>> { }


    public class ResendConfirmEmailRequestHandler : IRequestHandler<ResendConfirmEmailRequest, ServiceResult<string>>
    {
        private readonly IAccountService _accountService;
        public ResendConfirmEmailRequestHandler(IAccountService accountService)
        {
            _accountService = accountService;
        }
        public async Task<ServiceResult<string>> Handle(ResendConfirmEmailRequest request, CancellationToken cancellationToken)
        {
            var result = await _accountService.ResendConfirmationEmailAsync(request.model , request.origin);

            return result;
        }
    }
}
