using DB.DBcontext;
using Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShowTickets.Ticketmodels.User;
using User.Password;

namespace User.Registration
{
    public class UserRegistrationService
    {
        private readonly ShowDbContext _context;

        public UserRegistrationService(ShowDbContext dbContext)
        {
            _context = dbContext;
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
            _context.Add(newuser);
            await _context.SaveChangesAsync();

            // Return success response
            return new OkObjectResult(new { Message = "User registered successfully." });
        }
    }
}
