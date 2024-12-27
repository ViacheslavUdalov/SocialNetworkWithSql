using ASP.SecondSocialWithSQL.DTOS;
using ASP.SecondSocialWithSQL.Entities;
using ASP.SecondSocialWithSQL.Helpers;

namespace ASP.SecondSocialWithSQL.Interfaces;

public interface IMessageRepository
{
    void AddGroup(Group group);
    void RemoveConnection(Connection connection);
    Task<Connection> GetConnection(string connectionId);
    Task<Group> GetMessageGroup(string groupName);
    Task<Group> GetGroupForConnection(string connectionId);
    void AddMessage(Message message);
    void DeleteMessage(Message message);
    Task<Message> GetMessage(int id);
    Task<PageList<MessageDto>> GetMessagesForUser(MessageParams messageParams);
    Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientname);

}