using BookAPI.Entities;
using BookAPI.Models;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly BookDbContext context;
        private readonly IConfiguration configuration;

        public AuthController(BookDbContext context, IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
        }

        private string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                issuer: configuration["JWT:Issuer"],
                audience: configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMonths(1),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var user = await context.Users.FirstOrDefaultAsync(x => x.Username == request.Username && x.Password == request.Password && x.DeletedAt == null);
                if (user is null || user.Role != "user")
                {
                    return BadRequest(new
                    {
                        status = "Failed",
                        message = "Invalid Credentials"
                    });
                }
                return Ok(new
                {
                    user = user.Adapt<UserDto>(),
                    token = GenerateToken(user),

                });
            }
            catch (Exception ex)
            {
                return Problem(ex.ToString());
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var user = await context.Users.FirstOrDefaultAsync(x => x.Username == request.Username && x.DeletedAt == null);
                if (user is not null)
                {
                    return BadRequest(new
                    {
                        status = "Failed",
                        message = "Username already exists"
                    });
                }
                user = new User
                {
                    FullName = request.FullName,
                    Username = request.Username,
                    Password = request.Password,
                    Role = "user",
                    ImageProfile = "UserProfile/blank-profile-picture.png",
                    CreatedAt = DateTime.Now,
                };
                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();
                return Ok(new
                {
                    user = user.Adapt<UserDto>(),
                    token = GenerateToken(user),

                });
            }
            catch (Exception ex)
            {
                return Problem(ex.ToString());
            }
        }

        [Authorize(Roles = "user")]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                var user = await context.Users.FirstOrDefaultAsync(x => x.Id == userId && x.DeletedAt == null);
                if (user is null)
                {
                    return BadRequest(new
                    {
                        status = "Failed",
                        message = "User not found"
                    });
                }
                return Ok(new
                {
                    user = user.Adapt<UserDto>(),
                });
            }
            catch (Exception ex)
            {
                return Problem(ex.ToString());
            }
        }
    }
}
