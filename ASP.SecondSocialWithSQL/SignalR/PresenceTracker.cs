using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASP.SecondSocialWithSQL.SignalR;

public class PresenceTracker
{
    private static Dictionary<string, List<string>> OnlineUsers = new Dictionary<string, List<string>>();

    public Task<bool> UsersConnected(string username, string connectionId)
    {
        bool isOnline = false;
        lock (OnlineUsers)
        {
            if (OnlineUsers.ContainsKey(username))
            {
                OnlineUsers[username].Add(connectionId);
            }
            else
            {
                OnlineUsers.Add(username, new List<string> { connectionId });
                isOnline = true;
            }

            
        }
        return Task.FromResult(isOnline);
    }

    public Task<bool> UsersDisconnected(string username, string connectionId)
    {
        bool isOffline = false;
        lock (OnlineUsers)
        {
            if (!OnlineUsers.ContainsKey(username)) return Task.FromResult(isOffline);
            OnlineUsers[username].Remove(connectionId);
            if (OnlineUsers[username].Count == 0)
            {
                OnlineUsers.Remove(username);
                isOffline = true;
            }
        }

        return Task.FromResult(isOffline);
    }

    public Task<string[]> GetOnlineUsers()
    {
        string[] onlineUsers;
        lock (OnlineUsers)
        {
            onlineUsers = OnlineUsers.OrderBy(k => k.Key).Select(k => k.Key).ToArray();
        }

        return Task.FromResult(onlineUsers);
    }

    public Task<List<string>> GetConnectionsForUser(string username)
    {
        List<string> connectionIds;
        // В данном случае lock (OnlineUsers) устанавливает блокировку на объект OnlineUsers.
        // Это значит, что пока один поток выполняет код внутри этого блока, никакой другой поток
        // не может войти в этот блок с тем же объектом блокировки (OnlineUsers)
        lock (OnlineUsers)
        {
            connectionIds = OnlineUsers.GetValueOrDefault(username);
        }

// Task.FromResult(connectionIds) создает завершенный Task, который возвращает значение connectionIds.
// Это используется для того, чтобы вернуть значение в асинхронном методе без
// необходимости фактического выполнения асинхронной операции.
        return Task.FromResult(connectionIds);
    }
}