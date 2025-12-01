using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public interface IUserConnectionManager
    {
   void AddConnection(string userId, string connectionId);
        void RemoveConnection(string connectionId);
    List<string> GetConnections(string userId);
        bool IsUserConnected(string userId);
    }
}
