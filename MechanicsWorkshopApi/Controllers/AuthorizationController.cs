﻿using Microsoft.AspNetCore.Mvc;
using MechanicsWorkshopApi.Entities;
using MechanicsWorkshopApi.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

namespace MechanicsWorkshopApi.Controllers
{

    [Route("workshop/[controller]")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

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

            return Created($"/workshop/clients/{user.ID}", user); ;
        }


        [HttpPost("login")]
        public async Task<ActionResult<User>> Login(UserRequest request)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.Login == request.Username);

            if (user == null)
            {
                return NotFound("User not found!"); // for production only!
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return BadRequest("Invalid password!");
            }

            string token = CreateToken(user);

            return Ok(token);
        }


        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Login)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection(
                "REST_API_JWT_SERCRET_KEY").Value!));

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
