using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public record ConfirmEmailRequest(string userId, string token) : IRequest<ServiceResult<string>> { }


    public record ConfirmEmailRequestHandler : IRequestHandler<ConfirmEmailRequest, ServiceResult<string>>
    {
        private readonly IAccountService _accountService;

        public ConfirmEmailRequestHandler(IAccountService accountService)
        {
            _accountService = accountService;
        }
        public async Task<ServiceResult<string>> Handle(ConfirmEmailRequest request, CancellationToken cancellationToken)
        {
            var result = await _accountService.ConfirmEmailAsync(request.userId, request.token);

            return result;

        }
    }
}
