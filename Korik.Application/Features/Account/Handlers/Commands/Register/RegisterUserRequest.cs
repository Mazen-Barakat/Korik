using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public record RegisterUserRequest(RegisterDTO RegisterDTO, string origin) : IRequest<ServiceResult<UserDTO>> { }


    public record RegisterUserRequestHandler : IRequestHandler<RegisterUserRequest, ServiceResult<UserDTO>>
    {
        
        private readonly IAccountService _accountService;

        public RegisterUserRequestHandler
            (
                IAccountService accountService
            )
        {
            _accountService = accountService;
        }
        public async Task<ServiceResult<UserDTO>> Handle(RegisterUserRequest request, CancellationToken cancellationToken)
        {

            var result = await _accountService.RegisterAsync(request.RegisterDTO, request.origin);

            return result;  
        }
    }
}
