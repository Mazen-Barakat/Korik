using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public interface IAiAssistantService
    {
        /// <summary>
        /// Processes a natural language query from the user with conversation context.
        /// </summary>
        /// <param name="userId">The ID of the currently authenticated user</param>
        /// <param name="userMessage">The raw text input from the user</param>
        /// <param name="sessionId">Optional session ID to maintain conversation context</param>
        /// <returns>A structured response containing the AI's answer</returns>
        Task<ServiceResult<AiChatDTO>> ProcessQueryAsync(string userId, string userMessage, string? sessionId = null);

        /// <summary>
        /// Clears the conversation history for a user's session.
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <param name="sessionId">Optional session ID (clears default session if not provided)</param>
        void ClearConversation(string userId, string? sessionId = null);
    }
}
