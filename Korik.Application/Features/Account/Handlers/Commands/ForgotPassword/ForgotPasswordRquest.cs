using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public record ForgotPasswordRquest(ForgotPasswordDTO model , string origin) : IRequest<ServiceResult<string>> { }


    public class ForgotPasswordRquestHandler : IRequestHandler<ForgotPasswordRquest, ServiceResult<string>>
    {
        private readonly IAccountService _accountService;

        public ForgotPasswordRquestHandler(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task<ServiceResult<string>> Handle(ForgotPasswordRquest request, CancellationToken cancellationToken)
        {
            var result = await _accountService.ForgotPasswordAsync(request.model , request.origin);

            return result;
        }
    }


}
