using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RestaurantAPI_ASP.NET_Core.Entities;
using RestaurantAPI_ASP.NET_Core.Exceptions;
using RestaurantAPI_ASP.NET_Core.Model;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantAPI_ASP.NET_Core.Services
{
    public class AccountService:IAccountService
    {
        private RestaurantDbContext _context;
        private IPasswordHasher<User> _hasher;
        private AuthenticationSettings _settings;

        public AccountService(RestaurantDbContext context, IPasswordHasher<User> hasher, AuthenticationSettings settings)
        {
            _context = context;
            _hasher = hasher;
            _settings = settings;
        }

        public string GenerateJwt(LoginDto dto)
        {
            var user = _context.Users
                .Include(user => user.Role)
                .FirstOrDefault(u => u.Email == dto.Login);
            if(user is null)
            {
                throw new LoginException("Invalid login or password");
            }

            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if(result == PasswordVerificationResult.Failed)
            {
                throw new LoginException("Invalid login or password");
            }

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Role, $"{user.Role.Name}"),
                new Claim("DateOfBirth", user.DateOfBirth?.ToString("yyyy-MM-dd")),
                new Claim("Nationality", user.Nationality)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.JwtKey));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(_settings.JwtExpireDays);

            var token = new JwtSecurityToken(_settings.JwtIssuer,
                _settings.JwtIssuer,
                claims,
                expires: expires,
                signingCredentials: cred);

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }

        public void RegisterUser(RegisterUserDto dto)
        {
            var user = new User()
            {
                Email = dto.Email,
                Nationality = dto.Nationality,
                DateOfBirth = dto.DateOfBirth,
                PasswordHash = dto.Password,
                RoleId = dto.RoleId
            };
            
            user.PasswordHash = _hasher.HashPassword(user, dto.Password);
            _context.Users.Add(user);
            _context.SaveChanges();
        }
    }
}
