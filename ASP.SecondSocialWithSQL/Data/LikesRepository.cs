using ASP.SecondSocialWithSQL.DTOS;
using ASP.SecondSocialWithSQL.Entities;
using ASP.SecondSocialWithSQL.Extenstions;
using ASP.SecondSocialWithSQL.Helpers;
using ASP.SecondSocialWithSQL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ASP.SecondSocialWithSQL.Data;

public class LikesRepository : ILikesRepository
{
    private readonly DataContext _dataContext;

    public LikesRepository(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<UserLike> GetUserLike(int sourceUserId, int LikedUserId)
    {
        return await _dataContext.Likes.FindAsync(sourceUserId, LikedUserId);
    }

    public async Task<AppUser> GetUserWithLikes(int userId)
    {
        return await _dataContext.Users.Include(x => x.LikedUsers)
            .FirstOrDefaultAsync(x => x.Id == userId);
    }

    public async Task<PageList<LikeDto>> GetUserLikes(LikesParams likesParams)
    {
        // IEnumerable - это интерфейс в C#, который представляет коллекцию объектов,
        // которые можно перебирать с помощью цикла foreach. 
        // LINQ (Language Integrated Query) - это технология запросов, которая позволяет выполнять запросы к
        // данным в C# и других языках .NET. Она позволяет использовать выражения запросов для извлечения данных
        // из коллекций, баз данных и других источников данных. LINQ работает с коллекциями, реализующими интерфейс
        // IEnumerable. Для выполнения запросов к данным с использованием
        // LINQ можно использовать метод AsQueryable, который преобразует коллекцию IEnumerable в объект IQueryable,
        // позволяя выполнять отложенные запросы LINQ
        
        var users = _dataContext.Users.OrderBy(u => u.UserName).AsQueryable();
        var likes = _dataContext.Likes.AsQueryable();
        if (likesParams.Predicate == "liked")
        {
            likes = likes.Where(like => like.SourceUserId == likesParams.UserId);
            users = likes.Select(like => like.LikedUser);
        }

        if (likesParams.Predicate == "likedBy")
        {
            likes = likes.Where(like => like.LikedUserId == likesParams.UserId);
            users = likes.Select(like => like.SourceUser);
        }

      var likedUsers = users.Select(user => new LikeDto
        {
            Username = user.UserName,
            KnownAs = user.KnownAs,
            Age = user.DateOfBirth.CalculateAge(),
            PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url,
            City = user.City,
            Id = user.Id
        });
      return await PageList<LikeDto>.CreateAsync(likedUsers, likesParams.PageNumber, likesParams.PageSize);
    }
}