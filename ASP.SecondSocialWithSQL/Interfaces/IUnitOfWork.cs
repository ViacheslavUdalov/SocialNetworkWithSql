namespace ASP.SecondSocialWithSQL.Interfaces;

public interface IUnitOfWork
{
    IUserRepository _userRepository { get; }
    IMessageRepository _messageRepository { get; }
    ILikesRepository _likesRepository { get; }
    Task<bool> Complete();
    bool HasChanges();
}