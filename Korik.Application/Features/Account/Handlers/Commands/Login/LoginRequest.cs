using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public record LoginRequest(LoginDTO model) : IRequest<ServiceResult<UserDTO>> { }


    public class LoginRquestHandler : IRequestHandler<LoginRequest, ServiceResult<UserDTO>>
    {
        private readonly IAccountService _accountService;

        public LoginRquestHandler(IAccountService accountService)
        {
            _accountService = accountService;
        }
        public async Task<ServiceResult<UserDTO>> Handle(LoginRequest request, CancellationToken cancellationToken)
        {
            var result = await _accountService.LoginAsync(request.model);

            return result;
        }
    }

}
