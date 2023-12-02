using ASP.SecondSocialWithSQL.Etities;

namespace ASP.SecondSocialWithSQL.Interfaces;

public interface ITokenService
{
    string CreateToken(AppUser user); 
}