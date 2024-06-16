using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASP.SecondSocialWithSQL.SignalR;

public class PresenceTracker
{
    private static readonly Dictionary<string, List<string>> OnlineUsers = new Dictionary<string, List<string>>();

    public Task UserConnected(string username, string connectionId)
    {
        // Использование lock позволяет предотвратить конфликты доступа к этой коллекции, блокируя доступ
        // к ней для других потоков во время выполнения операций добавления или обновления данных.
        //Предполагается, что этот класс используется в контексте многопоточной среды
        //где могут одновременно выполняться запросы от разных пользователей
        lock (OnlineUsers)
        {
            if (OnlineUsers.ContainsKey(username))
            {
                OnlineUsers[username].Add(connectionId);
            }
            else
            {
                OnlineUsers.Add(username, new List<string>{connectionId});
            }
        }
        return Task.CompletedTask;
    }

    public Task UserDisconnected(string username, string connectionId)
    {
        lock (OnlineUsers)
        {
            if (!OnlineUsers.ContainsKey(username)) return Task.CompletedTask;
            OnlineUsers[username].Remove(connectionId);
            if (OnlineUsers[username].Count() == 0)
            {
                OnlineUsers.Remove(username);
            }
        }
        return Task.CompletedTask;
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
}