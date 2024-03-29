using System.Security.Cryptography;
using System.Text;
using ASP.SecondSocialWithSQL.Data;
using ASP.SecondSocialWithSQL.DTOS;
using ASP.SecondSocialWithSQL.Entities;
using ASP.SecondSocialWithSQL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASP.SecondSocialWithSQL.Controllers;
// [ApiController]
// [Route("api/[controller]")]
public class AccountController : BaseApiController
{
    private readonly DataContext _dataContext;
    private readonly ITokenService _tokenService;

    public AccountController(DataContext dataContext, ITokenService tokenService)
    {
        _dataContext = dataContext;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if (await UserExist(registerDto.UserName)) return BadRequest("UserName is taken");
            using var hmac = new HMACSHA512();
        var user = new AppUser
        {
            UserName = registerDto.UserName.ToLower(),
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
            PasswordSalt = hmac.Key
        };
        _dataContext.Users.Add(user);
        await _dataContext.SaveChangesAsync();
        return new UserDto
        {
            UserName = user.UserName,
            Token = _tokenService.CreateToken(user)
        };
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto logindto)
    {
        var user = await _dataContext.Users.SingleOrDefaultAsync(x => x.UserName == logindto.UserName.ToLower());
        if (user == null) return Unauthorized("Invalid username");
        using var hmac = new HMACSHA512(user.PasswordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(logindto.Password));
        for (int i = 0; i < computedHash.Length; i++)
        {
            if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("invalid Password");
        }

        return new UserDto
        {
            UserName = user.UserName,
            Token = _tokenService.CreateToken(user)
        };
    }
    private async Task<bool> UserExist(string username)
    {
        return await _dataContext.Users.AnyAsync(x => x.UserName == username.ToLower());
    }
}