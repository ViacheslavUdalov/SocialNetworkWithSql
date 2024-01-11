using ASP.SecondSocialWithSQL.Entities;

namespace ASP.SecondSocialWithSQL.Interfaces;

public interface ITokenService
{
    string CreateToken(AppUser user); 
}