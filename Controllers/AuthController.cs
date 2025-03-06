using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using TaskManagementAPI.Models;
using System.Configuration;

[ApiController]
[Route("auth/")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;

    public AuthController(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserDto userDto)
    {
        var hasher = new PasswordHasher<User>();
        var user = new User { Username = userDto.Username };
        user.PasswordHash = hasher.HashPassword(user, userDto.Password);

        _db.Users.Add(user);
        await _db.SaveChangesAsync(); 

        return Ok("User registered successfully");
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserDto userDto)
    {
        var user = await _db.Users.SingleOrDefaultAsync(u => u.Username == userDto.Username);
        if (user == null)
        {
            return Unauthorized("Invalid username or password.");
        }

        var hasher = new PasswordHasher<User>();
        var result = hasher.VerifyHashedPassword(user, user.PasswordHash, userDto.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            return Unauthorized("Invalid username or password.");
        }


        var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is missing"));
        var issuer = _config["Jwt:Issuer"];
        var audience = _config["Jwt:Audience"];
        var tokenExpiryHours = int.Parse(_config["Jwt:TokenExpiryHours"] ?? "24");

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        }),
            Expires = DateTime.UtcNow.AddHours(tokenExpiryHours),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return Ok(new { Token = tokenHandler.WriteToken(token) });
    }


}

