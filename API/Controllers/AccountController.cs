using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
  public class AccountController : BaseApiControllre
  {
    private readonly DataContext _context;
    private readonly ITokenService _tokenService;
    public AccountController(DataContext context, ITokenService tokenService)
    {
      _tokenService = tokenService;
      _context = context;
    }

    [HttpPost("register")]

    public async Task<ActionResult<UsreDto>> Register(RegisterDto registerDto)
    {
      if (await UserExists(registerDto.Username)) return BadRequest("username is taken");
      using var hmac = new HMACSHA512();

      var user = new AppUser
      {
        UserName = registerDto.Username.ToLower(),
        PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
        PasswordSald = hmac.Key
      };
      _context.Users.Add(user);
      await _context.SaveChangesAsync();

      return new UsreDto{
        Username=user.UserName,
        Token=_tokenService.CreateToken(user)
      };
    }
    [HttpPost("login")]
    public async Task<ActionResult<UsreDto>> Login(LoginDto loginDto)
    {
      var user = await _context.Users
          .SingleOrDefaultAsync(ECKeyXmlFormat => ECKeyXmlFormat.UserName == loginDto.Username);
      if (user == null) return Unauthorized("Invalid UserName");

      using var hmac = new HMACSHA512(user.PasswordSald);

      var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

      for (int i = 0; i < computedHash.Length; i++)
      {
        if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
      }
      return new UsreDto{
        Username=user.UserName,
        Token=_tokenService.CreateToken(user)
      };
    }
    private async Task<bool> UserExists(string username)
    {
      return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
    }
  }
}