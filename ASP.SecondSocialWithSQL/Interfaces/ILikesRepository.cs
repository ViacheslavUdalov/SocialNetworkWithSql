using ASP.SecondSocialWithSQL.DTOS;
using ASP.SecondSocialWithSQL.Entities;
using ASP.SecondSocialWithSQL.Helpers;

namespace ASP.SecondSocialWithSQL.Interfaces;

public interface ILikesRepository
{
    Task<UserLike> GetUserLike(int sourceUserId, int LikedUserId);
    Task<AppUser> GetUserWithLikes(int userId);
    Task<PageList<LikeDto>> GetUserLikes(LikesParams likesParams);
}