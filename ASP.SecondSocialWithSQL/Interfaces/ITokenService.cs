using ASP.SecondSocialWithSQL.Entities;

namespace ASP.SecondSocialWithSQL.Interfaces;

public interface ITokenService
{
    Task<string> CreateToken(AppUser user); 
}