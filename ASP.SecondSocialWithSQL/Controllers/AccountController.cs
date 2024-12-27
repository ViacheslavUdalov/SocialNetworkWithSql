using ASP.SecondSocialWithSQL.Data;
using ASP.SecondSocialWithSQL.DTOS;
using ASP.SecondSocialWithSQL.Entities;
using ASP.SecondSocialWithSQL.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASP.SecondSocialWithSQL.Controllers;
public class AccountController : BaseApiController
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;

    public AccountController(UserManager<AppUser> userManager, 
        SignInManager<AppUser> signInManager, 
        ITokenService tokenService, 
        IMapper mapper)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _mapper = mapper;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if (await UserExist(registerDto.UserName)) return BadRequest("UserName is taken");
        
        var user = _mapper.Map<AppUser>(registerDto);
        

            user.UserName = registerDto.UserName.ToLower();

           var result = await _userManager.CreateAsync(user, registerDto.Password);

           if (!result.Succeeded) return BadRequest(result.Errors);

           var roleResult = await _userManager.AddToRoleAsync(user, "Member");

           if (!roleResult.Succeeded) return BadRequest(result.Errors);
           
           return new UserDto
        {
            UserName = user.UserName,
            Token = await _tokenService.CreateToken(user),
            KnownAs = user.KnownAs,
            Gender = user.Gender
        };
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto logindto)
    {
        var user = await _userManager.Users
            .Include(p => p.Photos)
            .SingleOrDefaultAsync(x => x.UserName == logindto.UserName.ToLower());
        if (user == null) return Unauthorized("Invalid username");

        var result = await _signInManager.CheckPasswordSignInAsync(user, logindto.Password, false);
        if (!result.Succeeded) return Unauthorized();
        
        return new UserDto
        {
            UserName = user.UserName,
            Token = await _tokenService.CreateToken(user),
            PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
            KnownAs = user.KnownAs,
            Gender = user.Gender
        };
    }
    private async Task<bool> UserExist(string username)
    {
        return await _userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
    }
}