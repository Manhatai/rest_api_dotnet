using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Serilog;
using MechanicsWorkshopApi.Infra.Entities;
using MechanicsWorkshopApi.Infra.Data;

namespace MechanicsWorkshopApi.app.api.Security.Authorization
{

    [Route("workshop/[controller]")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly DataContext _context; // used to access DB
        private readonly IConfiguration _configuration; // used to access configuration settings (?)

        public AuthorizationController(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }


        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserRequest request)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Login = request.Username,
                PasswordHash = passwordHash,
            };

            _context.User.Add(user);
            await _context.SaveChangesAsync();

            Log.Information("New user created successfully [201]");
            return Created($"/workshop/clients/{user.ID}", user); ;
        }


        [HttpPost("login")]
        public async Task<ActionResult<User>> Login(UserRequest request)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.Login == request.Username);

            if (user == null)
            {
                Log.Information("Desired user not found (invalid login) [404]");
                return NotFound("Invalid login!"); // for development only!
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                Log.Information("Invalid password [400]");
                return BadRequest("Invalid password!");
            }

            string token = CreateToken(user);

            Log.Information("Token generated successfully [200]");
            return Ok(token);
        }


        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Login)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection(
                "REST_API_JWT_SK").Value!));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: cred
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}
