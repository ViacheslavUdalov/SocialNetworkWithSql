using ASP.SecondSocialWithSQL.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace ASP.SecondSocialWithSQL.Controllers;

[ServiceFilter(typeof(LogUserActivity))]
[ApiController]
[Route("api/[controller]")]
public class BaseApiController : ControllerBase
{
    
}