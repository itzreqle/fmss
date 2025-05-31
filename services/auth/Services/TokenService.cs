using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Roza.AuthService.Data;
using Roza.AuthService.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Roza.AuthService.Services
{
    public class TokenService
    {
        private readonly IConfiguration _config;
         private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
         private readonly RsaSecurityKey _signingKey;


        public TokenService(IConfiguration config, ApplicationDbContext context, UserManager<ApplicationUser> userManager, IPrivateRsaKeyProvider privateKeyProvider)
        {
            _config = config;
            _context = context;
            _userManager = userManager;
            _signingKey = privateKeyProvider.GetKey();

        }

        public async Task<string> GenerateTokenAsync(ApplicationUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Name, user.UserName),
        };

            claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

            // Load RSA private key
            var creds = new SigningCredentials(_signingKey, SecurityAlgorithms.RsaSha256); // For RSA

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public async Task<string> GenerateRefreshToken(ApplicationUser user)
        {
            var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

            var token = new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                ExpiryDate = DateTime.UtcNow.AddDays(7), // For example, 7 days validity
                IsRevoked = false
            };

            _context.RefreshTokens.Add(token);
            await _context.SaveChangesAsync();

            return refreshToken;
        }
       
    }
}