using Microsoft.AspNetCore.Identity;

namespace ASP.SecondSocialWithSQL.Entities;

public class AppRole : IdentityRole<int>
{
    public ICollection<AppUserRole> UserRoles { get; set; }
}