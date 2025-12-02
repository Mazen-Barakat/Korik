using Korik.Application;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Infrastructure
{
    public class UserConnectionManager : IUserConnectionManager
    {
     // ConcurrentDictionary: userId ? List of connectionIds (user can have multiple devices/tabs)
        private readonly ConcurrentDictionary<string, List<string>> _userConnections = new();

        public void AddConnection(string userId, string connectionId)
        {
             _userConnections.AddOrUpdate(userId,
                 // If key doesn't exist, create new list with connectionId
                        new List<string> { connectionId },
             // If key exists, add connectionId to existing list
                  (key, existingList) =>
                  {
                 lock (existingList)
                           {
                      if (!existingList.Contains(connectionId))
                    {
                  existingList.Add(connectionId);
                  }
                  }
                 return existingList;
            });
        }

        public void RemoveConnection(string connectionId)
        {
                // Find and remove the connectionId from any user's list
            foreach (var kvp in _userConnections)
            {
                lock (kvp.Value)
            {
                kvp.Value.Remove(connectionId);
            }

                // If user has no more connections, remove the user entry
            if (kvp.Value.Count == 0)
            {
                _userConnections.TryRemove(kvp.Key, out _);
            }
                }
        }

        public List<string> GetConnections(string userId)
                 {
                    if (_userConnections.TryGetValue(userId, out var connections))
                    {
                        lock (connections)
                    {
                          return new List<string>(connections);
                    }
                    }

                      return new List<string>();
                }

        public bool IsUserConnected(string userId)
             {
             return _userConnections.ContainsKey(userId) && _userConnections[userId].Count > 0;
          }
    }
}
