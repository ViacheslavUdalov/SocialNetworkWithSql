using ASP.SecondSocialWithSQL.DTOS;
using ASP.SecondSocialWithSQL.Entities;
using ASP.SecondSocialWithSQL.Helpers;
using ASP.SecondSocialWithSQL.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace ASP.SecondSocialWithSQL.Data;

public class MessageRepository : IMessageRepository
{
    private readonly DataContext _dataContext;
    private readonly IMapper _mapper;

    public MessageRepository(DataContext dataContext, IMapper mapper)
    {
        _dataContext = dataContext;
        _mapper = mapper;
    }

    public void AddMessage(Message message)
    {
        _dataContext.Messages.Add(message);
    }

    public void DeleteMessage(Message message)
    {
        _dataContext.Messages.Remove(message);
    }

    public async Task<Message> GetMessage(int id)
    {
        return await _dataContext.Messages.FindAsync(id);
    }

    public async Task<PageList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
    {
        // Создание запроса к бд
        var query = _dataContext.Messages.OrderByDescending(m => m.MessageSend).AsQueryable();
        query = messageParams.Container switch
        {
            "Inbox" => query.Where(u => u.Recipient.UserName == messageParams.Username),
            "Outbox" => query.Where(u => u.Sender.UserName == messageParams.Username),
            _ => query.Where(u => u.Sender.UserName == messageParams.Username && u.DateRead == null)
        };
        var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);
        return await PageList<MessageDto>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
    }

    public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientname)
    {
        var messages = await _dataContext.Messages
            .Include(u => u.Sender).ThenInclude(u => u.Photos)
            .Include(u => u.Recipient).ThenInclude(u => u.Photos)
            .Where(m => m.Recipient.UserName == currentUsername
                        && m.Sender.UserName == recipientname
                        || m.Sender.UserName == currentUsername &&
                        m.Recipient.UserName == recipientname)
            .OrderBy(m => m.MessageSend)
            .ToListAsync();

        var unreadMessages = messages
            .Where(m => m.DateRead == null
                        && m.Recipient.UserName == currentUsername).ToList();
        if (unreadMessages.Any())
        {
            foreach (var message in unreadMessages)
            {
                message.DateRead = DateTime.Now;
            }

            await _dataContext.SaveChangesAsync();
        }

        return _mapper.Map<IEnumerable<MessageDto>>(messages);
    }

    public async Task<bool> SaveAllAsync()
    {
        return await _dataContext.SaveChangesAsync() > 0;
    }
}