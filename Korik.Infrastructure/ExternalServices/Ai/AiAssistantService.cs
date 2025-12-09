using Korik.Application;
using Korik.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace Korik.Infrastructure
{
    public class AiAssistantService : IAiAssistantService
    {
        private readonly Kernel _kernel;
        private readonly IChatCompletionService _chatCompletionService;
        private readonly IChatContextManager _chatContextManager;
        private readonly int _maxHistoryMessages;

        public AiAssistantService(
            IConfiguration configuration,
            IServiceProvider serviceProvider,
            IChatContextManager chatContextManager)
        {
            var builder = Kernel.CreateBuilder();

            builder.AddOpenAIChatCompletion(
                modelId: configuration["AI:ModelId"]!,
                apiKey: configuration["AI:ApiKey"]!
            );

            _kernel = builder.Build();
            _chatContextManager = chatContextManager;
            _maxHistoryMessages = configuration.GetValue("AI:MaxHistoryMessages", 20);

            RegisterPlugins(serviceProvider);

            _chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
        }

        private void RegisterPlugins(IServiceProvider serviceProvider)
        {
            // ═══════════════════════════════════════════════════════════════
            // BOOKING & APPOINTMENTS
            // ═══════════════════════════════════════════════════════════════
            var bookingPlugin = new BookingPlugin(
                serviceProvider.GetRequiredService<IBookingRepository>(),
                serviceProvider.GetRequiredService<ICarOwnerProfileRepository>(),
                serviceProvider.GetRequiredService<IWorkShopProfileRepository>());

            // ═══════════════════════════════════════════════════════════════
            // CAR MANAGEMENT
            // ═══════════════════════════════════════════════════════════════
            var carPlugin = new CarPlugin(
                serviceProvider.GetRequiredService<ICarRepository>(),
                serviceProvider.GetRequiredService<ICarOwnerProfileRepository>());

            var carMaintenancePlugin = new CarMaintenancePlugin(
                serviceProvider.GetRequiredService<ICarIndicatorRepository>(),
                serviceProvider.GetRequiredService<ICarRepository>(),
                serviceProvider.GetRequiredService<ICarOwnerProfileRepository>());

            var expensePlugin = new ExpensePlugin(
                serviceProvider.GetRequiredService<ICarExpenseRepository>(),
                serviceProvider.GetRequiredService<ICarRepository>(),
                serviceProvider.GetRequiredService<ICarOwnerProfileRepository>());

            // ═══════════════════════════════════════════════════════════════
            // WORKSHOPS & SERVICES
            // ═══════════════════════════════════════════════════════════════
            var workshopPlugin = new WorkshopPlugin(
                serviceProvider.GetRequiredService<IWorkShopProfileRepository>(),
                serviceProvider.GetRequiredService<IWorkshopServiceRepository>(),
                serviceProvider.GetRequiredService<IReviewRepository>());

            var servicePlugin = new ServicePlugin(
                serviceProvider.GetRequiredService<IServiceRepository>(),
                serviceProvider.GetRequiredService<ICategoryRepository>(),
                serviceProvider.GetRequiredService<ISubcategoryRepository>(),
                serviceProvider.GetRequiredService<IWorkshopServiceRepository>());

            // ═══════════════════════════════════════════════════════════════
            // NOTIFICATIONS
            // ═══════════════════════════════════════════════════════════════
            var notificationPlugin = new NotificationPlugin(
                serviceProvider.GetRequiredService<INotificationRepository>());

            // ═══════════════════════════════════════════════════════════════
            // USER PROFILE & ACCOUNT
            // ═══════════════════════════════════════════════════════════════
            var userProfilePlugin = new UserProfilePlugin(
                serviceProvider.GetRequiredService<ICarOwnerProfileRepository>(),
                serviceProvider.GetRequiredService<IWorkShopProfileRepository>(),
                serviceProvider.GetRequiredService<UserManager<ApplicationUser>>(),
                serviceProvider.GetRequiredService<ICarRepository>(),
                serviceProvider.GetRequiredService<IBookingRepository>(),
                serviceProvider.GetRequiredService<IReviewRepository>());

            // ═══════════════════════════════════════════════════════════════
            // REVIEWS & RATINGS
            // ═══════════════════════════════════════════════════════════════
            var reviewPlugin = new ReviewPlugin(
                serviceProvider.GetRequiredService<IReviewRepository>(),
                serviceProvider.GetRequiredService<ICarOwnerProfileRepository>(),
                serviceProvider.GetRequiredService<IWorkShopProfileRepository>(),
                serviceProvider.GetRequiredService<IBookingRepository>());

            // ═══════════════════════════════════════════════════════════════
            // HELP & GUIDANCE
            // ═══════════════════════════════════════════════════════════════
            var helpPlugin = new HelpPlugin();

            // ═══════════════════════════════════════════════════════════════
            // REGISTER ALL PLUGINS TO KERNEL
            // ═══════════════════════════════════════════════════════════════
            _kernel.ImportPluginFromObject(bookingPlugin, "BookingPlugin");
            _kernel.ImportPluginFromObject(carPlugin, "CarPlugin");
            _kernel.ImportPluginFromObject(carMaintenancePlugin, "CarMaintenancePlugin");
            _kernel.ImportPluginFromObject(expensePlugin, "ExpensePlugin");
            _kernel.ImportPluginFromObject(workshopPlugin, "WorkshopPlugin");
            _kernel.ImportPluginFromObject(servicePlugin, "ServicePlugin");
            _kernel.ImportPluginFromObject(notificationPlugin, "NotificationPlugin");
            _kernel.ImportPluginFromObject(userProfilePlugin, "UserProfilePlugin");
            _kernel.ImportPluginFromObject(reviewPlugin, "ReviewPlugin");
            _kernel.ImportPluginFromObject(helpPlugin, "HelpPlugin");
        }

        public async Task<ServiceResult<AiChatDTO>> ProcessQueryAsync(string userId, string userMessage, string? sessionId = null)
        {
            try
            {
                // Get or create conversation history for this user/session
                var history = _chatContextManager.GetOrCreateHistory(userId, sessionId);

                // Add system message if this is a new conversation
                if (history.Count == 0)
                {
                    var systemPrompt = CreateSystemPrompt(userId);
                    history.AddSystemMessage(systemPrompt);
                }

                // Add the user's message to history
                history.AddUserMessage(userMessage);

                // Configure AI settings
                OpenAIPromptExecutionSettings settings = new()
                {
                    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
                };

                // Get AI response with full conversation context
                var result = await _chatCompletionService.GetChatMessageContentAsync(
                    history,
                    settings,
                    _kernel);

                // Add assistant response to history
                history.AddAssistantMessage(result.Content ?? "I could not generate a response.");

                // Trim history to prevent token overflow
                _chatContextManager.TrimHistory(userId, sessionId, _maxHistoryMessages);

                var responseDto = new AiChatDTO
                {
                    Response = result.Content ?? "I could not generate a response.",
                    IsSuccess = true,
                    ActionTaken = result.Metadata?.ContainsKey("ToolCalls") == true ? "ToolUsed" : null
                };

                return ServiceResult<AiChatDTO>.Ok(responseDto);
            }
            catch (Exception ex)
            {
                return ServiceResult<AiChatDTO>.Fail($"AI Error: {ex.Message}");
            }
        }

        public void ClearConversation(string userId, string? sessionId = null)
        {
            _chatContextManager.ClearHistory(userId, sessionId);
        }

        private static string CreateSystemPrompt(string userId)
        {
            return $"""
                You are Korik AI, a helpful car maintenance and workshop booking assistant.
                You are helping User ID: {userId}.
                
                You can help users with:
                - Viewing and managing their car bookings and appointments
                - Checking car maintenance status and indicators
                - Finding workshops and services
                - Tracking car expenses and fuel costs
                - Getting personalized maintenance recommendations
                - Viewing notifications and pending confirmations
                - Managing their profile and account information
                - Viewing and managing reviews
                - Getting help and maintenance tips
                
                IMPORTANT CONTEXT RULES:
                - Remember previous messages in this conversation
                - If user refers to "it", "that", "the car", "the first one", etc., use context from previous messages
                - If user asks follow-up questions, use information from earlier in the conversation
                - When users ask about their data, use the available tools to fetch real information
                - If you need to call a function that requires userId, use: {userId}
                
                Be helpful, concise, and proactive. Format responses clearly with sections and bullet points.
                """;
        }
    }
}
