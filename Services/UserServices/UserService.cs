using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DB.DBcontext;
using Dtos;
using JWTAuthServer.DTOs;
using JWTAuthServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ShowTickets.Ticketmodels.User;
using User.Password;

namespace User.Registration
{
    public class UserRegistrationService
    {
        private readonly ShowDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly TokenService _tokenService;

        public UserRegistrationService(ShowDbContext dbContext, IConfiguration configuration, TokenService tokenService)
        {
            _context = dbContext;
            _configuration = configuration;
            _tokenService = tokenService;
        }

        public async Task<IActionResult> Createuser(RegistrationDTO registrationDTO)
        {
            if (registrationDTO == null)
            {
                return new BadRequestObjectResult("Registration data is required.");
            }

            // Check if email already exists
            bool emailExists = await _context.Users.AnyAsync(e => e.Email == registrationDTO.Email);
            if (emailExists)
            {
                return new ConflictObjectResult("Email is already in use.");
            }

            // Create new user object
            var newuser = new Users
            {
                Name = registrationDTO.Name,
                Email = registrationDTO.Email,
                PhoneNumber = registrationDTO.Phone
            };

            // Hash the password
            PasswordHasher.CreatePasswordHash(registrationDTO.Password, out byte[] passwordHash, out byte[] passwordSalt);
            newuser.PasswordHash = passwordHash;
            newuser.PasswordSalt = passwordSalt;

            // Save to database
            _context.Users.Add(newuser);
            await _context.SaveChangesAsync();

            // Return success response
            return new OkObjectResult(new { Message = "User registered successfully." });
        }

        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (loginDto == null)
            {
                return new BadRequestObjectResult("Login data insufficient.");
            }

            var _user = await _context.Users.FirstOrDefaultAsync(e => e.Email == loginDto.Email);

            if (_user == null)
            {
                return new NotFoundObjectResult("User Not Found.");
            }

            // Verify the password
            if (!PasswordHasher.VerifyPasswordHash(loginDto.Password, _user.PasswordHash, _user.PasswordSalt))
            {
                return new UnauthorizedObjectResult(new { message = "Invalid email or password." });
            }

            var accessToken = _tokenService.GenerateAccessToken(_user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            _user.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return new OkObjectResult(new { AccessToken = accessToken, RefreshToken = refreshToken.Token, Role = _user.Role });
        }

        public async Task<IActionResult> Logout(RefreshDTO refreshDto)
        {
            if (refreshDto == null || string.IsNullOrEmpty(refreshDto.RefreshToken))
            {
                return new BadRequestObjectResult("Refresh token is required.");
            }

            var refreshToken = await _context.RefreshToken
                .FirstOrDefaultAsync(rt => rt.Token == refreshDto.RefreshToken);

            if (refreshToken == null)
            {
                return new BadRequestObjectResult("Invalid refresh token.");
            }

            _context.RefreshToken.Remove(refreshToken); // Remove the refresh token from DB
            await _context.SaveChangesAsync();

            return new OkObjectResult(new { message = "Logged out successfully." });
        }

        public async Task<IActionResult> Refresh(RefreshDTO model)
        {
            if (model == null || string.IsNullOrEmpty(model.RefreshToken))
            {
                return new BadRequestObjectResult("Refresh token is required.");
            }

            var user = await _context.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == model.RefreshToken));

            if (user == null)
            {
                return new UnauthorizedObjectResult("Invalid refresh token.");
            }

            var oldToken = user.RefreshTokens.First(t => t.Token == model.RefreshToken);
            if (oldToken.IsRevoked || oldToken.Expires < DateTime.UtcNow)
            {
                return new UnauthorizedObjectResult("Token expired or revoked.");
            }

            var newAccessToken = _tokenService.GenerateAccessToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            oldToken.IsRevoked = true; // Mark old refresh token as revoked
            user.RefreshTokens.Add(newRefreshToken);

            await _context.SaveChangesAsync();

            return new OkObjectResult(new { AccessToken = newAccessToken, RefreshToken = newRefreshToken.Token });
        }
    }
}
