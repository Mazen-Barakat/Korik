using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public record ResetPasswordRequest(ResetPasswordDTO model) : IRequest<ServiceResult<string>> { }


    public class ResetPasswordRequestHandler : IRequestHandler<ResetPasswordRequest , ServiceResult<string>>
    {
        private readonly IAccountService _accountService;

        public ResetPasswordRequestHandler(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task<ServiceResult<string>> Handle(ResetPasswordRequest request, CancellationToken cancellationToken)
        {
            var result = await _accountService.ResetPasswordAsync(request.model);

            return result;

        }
    }
}
