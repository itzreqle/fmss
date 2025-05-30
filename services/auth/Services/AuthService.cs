using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Roza.AuthService.Data;
using Roza.AuthService.Models;

namespace Roza.AuthService.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TokenService _tokenService;
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            TokenService tokenService,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _context = context;
        }

        public async Task<AuthResult> RegisterAsync(RegisterModel model)
        {
            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return AuthResult.Failed(string.Join(", ", result.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(user, "User");
            return AuthResult.Success(null);
        }

        public async Task<AuthResult> LoginAsync(LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                return AuthResult.Failed("ایمیل یا رمز عبور اشتباه است");

            var accessToken = await _tokenService.GenerateTokenAsync(user);
            var refreshToken = await _tokenService.GenerateRefreshToken(user);

            return AuthResult.Success(accessToken, refreshToken);
        }

        public async Task<AuthResult> RefreshTokenAsync(string refreshToken)
        {
            var storedToken = await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken && !rt.IsRevoked);

            if (storedToken == null || storedToken.ExpiryDate < DateTime.UtcNow)
                return AuthResult.Failed("توکن نامعتبر یا منقضی شده است.");

            var newAccessToken = await _tokenService.GenerateTokenAsync(storedToken.User);
            var newRefreshToken = await _tokenService.GenerateRefreshToken(storedToken.User);

            storedToken.IsRevoked = true;
            await _context.SaveChangesAsync();

            return AuthResult.Success(newAccessToken, newRefreshToken);
        }
    }

    public interface IAuthService
    {
        Task<AuthResult> RegisterAsync(RegisterModel model);
        Task<AuthResult> LoginAsync(LoginModel model);
        Task<AuthResult> RefreshTokenAsync(string refreshToken);
    }


    public class AuthResult
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

        public static AuthResult Success(string accessToken, string refreshToken = null)
        {
            return new AuthResult
            {
                Succeeded = true,
                Message = "عملیات موفق بود",
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public static AuthResult Failed(string message)
        {
            return new AuthResult
            {
                Succeeded = false,
                Message = message
            };
        }
    }


}
