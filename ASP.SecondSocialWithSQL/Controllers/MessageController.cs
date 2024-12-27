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
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public MessageController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
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

        var sender = await _unitOfWork._userRepository.GetUserByUsernameAsync(username);
        var recipient = await _unitOfWork._userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);
        if (recipient == null) return NotFound();
        var message = new Message
        {
            Sender = sender,
            Recipient = recipient,
            SenderUsername = sender.UserName,
            RecipientUsername = recipient.UserName,
            Content = createMessageDto.Content
        };
            _unitOfWork._messageRepository.AddMessage(message);
        if (await _unitOfWork.Complete()) return Ok(_mapper.Map<MessageDto>(message));
        return BadRequest("Failed to save Message");
    }

    [HttpGet]
    public async Task<ActionResult<PageList<MessageDto>>> GetMessageForUser([FromQuery] MessageParams messageParams)
    {
        messageParams.Username = User.GetUsername();
        var messages = await _unitOfWork._messageRepository.GetMessagesForUser(messageParams);
        Response.AddPaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages);
        return messages;
    }
    
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMessage(int id)
    {
        var username = User.GetUsername();
        var message = await _unitOfWork._messageRepository.GetMessage(id);
        if (message.Sender.UserName != username && message.Recipient.UserName != username)
        {
            return Unauthorized();
        }

        if (message.Sender.UserName == username) message.SenderDeleted = true;
        if (message.Recipient.UserName == username) message.RecipientDeleted = true;
        if (message.SenderDeleted && message.RecipientDeleted) _unitOfWork._messageRepository.DeleteMessage(message);
        if (await _unitOfWork.Complete()) return Ok();
        return BadRequest("Не Удалось удалить сообщение");
    }
}