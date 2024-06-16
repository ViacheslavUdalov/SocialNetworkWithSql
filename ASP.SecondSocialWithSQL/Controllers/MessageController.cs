using ASP.SecondSocialWithSQL.DTOS;
using ASP.SecondSocialWithSQL.Entities;
using ASP.SecondSocialWithSQL.Extenstions;
using ASP.SecondSocialWithSQL.Helpers;
using ASP.SecondSocialWithSQL.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASP.SecondSocialWithSQL.Controllers;

[Authorize]
public class MessageController : BaseApiController
{
    private readonly IMessageRepository _messageRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public MessageController(IMessageRepository messageRepository, IUserRepository userRepository, IMapper mapper)
    {
        _messageRepository = messageRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDTO createMessageDto)
    {
        var username = User.GetUsername();
        if (username == createMessageDto.RecipientUsername.ToLower())
        {
            return BadRequest("Нельзя отправить сообщение себе!");
        }

        var sender = await _userRepository.GetUserByUsernameAsync(username);
        var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);
        if (recipient == null) return NotFound();
        var message = new Message
        {
            Sender = sender,
            Recipient = recipient,
            SenderUsername = sender.UserName,
            RecipientUsername = recipient.UserName,
            Content = createMessageDto.Content
        };
        _messageRepository.AddMessage(message);
        if (await _messageRepository.SaveAllAsync()) return Ok(_mapper.Map<MessageDto>(message));
        return BadRequest("Failed to save Message");
    }

    [HttpGet]
    public async Task<ActionResult<PageList<MessageDto>>> GetMessageForUser([FromQuery] MessageParams messageParams)
    {
        messageParams.Username = User.GetUsername();
        var messages = await _messageRepository.GetMessagesForUser(messageParams);
        Response.AddPaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages);
        return messages;
    }

    [HttpGet("thread/{username}")]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesThread(string username)
    {
        var currentUsername = User.GetUsername();
        return Ok(await _messageRepository.GetMessageThread(currentUsername, username));
    }
}