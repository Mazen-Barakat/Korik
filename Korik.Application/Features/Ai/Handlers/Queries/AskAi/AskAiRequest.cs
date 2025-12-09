using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public record AskAiRequest : IRequest<ServiceResult<AiChatDTO>>
    {
        public string UserId { get; init; }
        public string UserMessage { get; init; }
    }

    public class AskAiHandler : IRequestHandler<AskAiRequest, ServiceResult<AiChatDTO>>
    {
        private readonly IAiAssistantService _aiAssistantService;

        public AskAiHandler(IAiAssistantService aiAssistantService)
        {
            _aiAssistantService = aiAssistantService;
        }

        public async Task<ServiceResult<AiChatDTO>> Handle(AskAiRequest request, CancellationToken cancellationToken)
        {
            return await _aiAssistantService.ProcessQueryAsync(request.UserId, request.UserMessage);
        }
    }
}
