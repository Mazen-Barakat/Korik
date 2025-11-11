using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public record RefreshTokenRequest(RefreshTokenDTO model) : IRequest<ServiceResult<UserDTO>> { }

    public class RefreshTokenRequestHandler : IRequestHandler<RefreshTokenRequest, ServiceResult<UserDTO>>
    {
        private readonly IAccountService _accountService;

        public RefreshTokenRequestHandler(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task<ServiceResult<UserDTO>> Handle(RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            var result = await _accountService.RefreshTokenAsync(request.model);

            return result;
        }
    }
}
