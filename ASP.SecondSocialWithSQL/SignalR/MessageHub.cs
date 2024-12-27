using ASP.SecondSocialWithSQL.DTOS;
using ASP.SecondSocialWithSQL.Entities;
using ASP.SecondSocialWithSQL.Extenstions;
using ASP.SecondSocialWithSQL.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace ASP.SecondSocialWithSQL.SignalR;

public class MessageHub : Hub
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHubContext<PresenceHub> _hubClients;
    private readonly PresenceTracker _presenceTracker;

    public MessageHub( IMapper mapper, 
        IUnitOfWork unitOfWork, IHubContext<PresenceHub> hubClients, PresenceTracker presenceTracker)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _hubClients = hubClients;
        _presenceTracker = presenceTracker;
    }

    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var otherUser = httpContext.Request.Query["user"].ToString();
        var groupName = GetGroupName(Context.User.GetUsername(), otherUser);
        
        Console.WriteLine(Context.User.GetUsername());
        Console.WriteLine(Context.ConnectionId);
        
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        var group = await AddToGroup(groupName);
        await Clients.Group(groupName).SendAsync("UpdatedGroup", group);
        
        var messages = _unitOfWork._messageRepository.GetMessageThread(Context.User.GetUsername(), otherUser);

        if (_unitOfWork.HasChanges())
        {
            await _unitOfWork.Complete(); 
        }
        
        await Clients.Caller.SendAsync("ReceiveMessageThread", messages);
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var group = await RemoveFromMessageGroup();
        await Clients.Group(group.Name).SendAsync("UpdateGroup", group);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(CreateMessageDTO createMessageDto)
    {
        var username = Context.User.GetUsername();
        if (username == createMessageDto.RecipientUsername.ToLower())
        {
            throw new HubException("Нельзя отправить сообщение себе!");
        }

        var sender = await _unitOfWork._userRepository.GetUserByUsernameAsync(username);
        var recipient = await _unitOfWork._userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);
        if (recipient == null) throw new HubException("пользователь не найден");
        var message = new Message
        {
            Sender = sender,
            Recipient = recipient,
            SenderUsername = sender.UserName,
            RecipientUsername = recipient.UserName,
            Content = createMessageDto.Content
        };
        
        var groupName = GetGroupName(sender.UserName, recipient.UserName);
        var group = await _unitOfWork._messageRepository.GetMessageGroup(groupName);
        if (group.Connections.Any(x => x.Username == recipient.UserName))
        {
            message.DateRead = DateTime.UtcNow;
        }
        else
        {
            var connections = await _presenceTracker.GetConnectionsForUser(recipient.UserName);
            if (connections != null)
            {
                await _hubClients.Clients.Clients(connections).SendAsync("NewMessageReceived", new {username = sender.UserName, knowAs = sender.KnownAs});
            }
        }
        
        _unitOfWork._messageRepository.AddMessage(message);
        
        if (await _unitOfWork.Complete())
        {
            await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
        }
    }

    private async Task<Group> AddToGroup(string groupName)
    {
        var group = await _unitOfWork._messageRepository.GetMessageGroup(groupName);
        var connection = new Connection(Context.ConnectionId, Context.User.GetUsername());
        if (group == null)
        {
            group = new Group(groupName);
            _unitOfWork._messageRepository.AddGroup(group);
        }
        group.Connections.Add(connection);
        if (await _unitOfWork.Complete()) return group;
        throw new HubException("Не получилось войти в группу");
    }

    private async Task<Group> RemoveFromMessageGroup()
    {
        var group = await _unitOfWork._messageRepository.GetGroupForConnection(Context.ConnectionId);
        var connection = group.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
        _unitOfWork._messageRepository.RemoveConnection(connection);
        if (await _unitOfWork.Complete()) return group;
        throw new HubException("Не получилось выйти из группы");
    }

    private string GetGroupName(string caller, string other)
    {
        var stringCompare = string.CompareOrdinal(caller, other) < 0;
        return stringCompare ? $"@{caller}-{other}" : $"@{other}-{caller}";
    }
}