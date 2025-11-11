using Korik.Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public record SetPasswordRequest(SetPasswordDTO model , ClaimsPrincipal user ) : IRequest<ServiceResult<string>>
    {
    }


    public class SetPasswordRequestHandler : IRequestHandler<SetPasswordRequest, ServiceResult<string>>
    {
        private readonly IAccountService _accountService;
        public SetPasswordRequestHandler(IAccountService accountService)
        {
            _accountService = accountService;
        }
        public async Task<ServiceResult<string>> Handle(SetPasswordRequest request, CancellationToken cancellationToken)
        {
            var result = await _accountService.SetPasswordAsync(request.model , request.user);
            return result;
        }
    }
}
