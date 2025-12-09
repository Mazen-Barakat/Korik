using Microsoft.SemanticKernel.ChatCompletion;
using System.Collections.Concurrent;

namespace Korik.Infrastructure
{
    /// <summary>
    /// Manages chat conversation history for users to maintain context across messages.
    /// </summary>
    public interface IChatContextManager
    {
        ChatHistory GetOrCreateHistory(string userId, string? sessionId = null);
        void ClearHistory(string userId, string? sessionId = null);
        IEnumerable<string> GetUserSessions(string userId);
        void TrimHistory(string userId, string? sessionId = null, int maxMessages = 20);
    }

    /// <summary>
    /// In-memory implementation of chat context manager.
    /// For production, consider using Redis or a database.
    /// </summary>
    public class InMemoryChatContextManager : IChatContextManager
    {
        private readonly ConcurrentDictionary<string, ChatSession> _sessions = new();
        private readonly TimeSpan _sessionTimeout;

        public InMemoryChatContextManager(int maxMessagesPerSession = 20, int sessionTimeoutMinutes = 30)
        {
            _sessionTimeout = TimeSpan.FromMinutes(sessionTimeoutMinutes);
        }

        public ChatHistory GetOrCreateHistory(string userId, string? sessionId = null)
        {
            var key = GetKey(userId, sessionId);

            var session = _sessions.AddOrUpdate(
                key,
                _ => new ChatSession
                {
                    History = new ChatHistory(),
                    LastAccessed = DateTime.UtcNow,
                    UserId = userId,
                    SessionId = sessionId
                },
                (_, existing) =>
                {
                    existing.LastAccessed = DateTime.UtcNow;
                    return existing;
                });

            CleanupExpiredSessions();
            return session.History;
        }

        public void ClearHistory(string userId, string? sessionId = null)
        {
            var key = GetKey(userId, sessionId);
            _sessions.TryRemove(key, out _);
        }

        public IEnumerable<string> GetUserSessions(string userId)
        {
            return _sessions
                .Where(kvp => kvp.Value.UserId == userId)
                .Select(kvp => kvp.Value.SessionId ?? "default")
                .ToList();
        }

        public void TrimHistory(string userId, string? sessionId = null, int maxMessages = 20)
        {
            var key = GetKey(userId, sessionId);

            if (_sessions.TryGetValue(key, out var session))
            {
                var history = session.History;

                // Keep system message + last N messages
                while (history.Count > maxMessages + 1)
                {
                    for (int i = 0; i < history.Count; i++)
                    {
                        if (history[i].Role != AuthorRole.System)
                        {
                            history.RemoveAt(i);
                            break;
                        }
                    }
                }
            }
        }

        private static string GetKey(string userId, string? sessionId)
        {
            return string.IsNullOrEmpty(sessionId) ? userId : $"{userId}:{sessionId}";
        }

        private void CleanupExpiredSessions()
        {
            var now = DateTime.UtcNow;
            var expiredKeys = _sessions
                .Where(kvp => now - kvp.Value.LastAccessed > _sessionTimeout)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var key in expiredKeys)
            {
                _sessions.TryRemove(key, out _);
            }
        }

        private class ChatSession
        {
            public ChatHistory History { get; set; } = new();
            public DateTime LastAccessed { get; set; }
            public string UserId { get; set; } = string.Empty;
            public string? SessionId { get; set; }
        }
    }
}
