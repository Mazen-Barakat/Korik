using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public record GoogleLoginRequest(GoogleLoginDTO model) : IRequest<ServiceResult<UserDTO>> { }


    public class GoogleLoginRequestHandler : IRequestHandler<GoogleLoginRequest , ServiceResult<UserDTO>>
    {
        private readonly IGoogleAuthService _googleAuthService;

        public GoogleLoginRequestHandler(IGoogleAuthService googleAuthService)
        {
            _googleAuthService = googleAuthService;
        }

        public async Task<ServiceResult<UserDTO>> Handle(GoogleLoginRequest request, CancellationToken cancellationToken)
        {
            var result = await _googleAuthService.GoogleLoginAsync(request.model);

            return result; 
        }
    }

}
