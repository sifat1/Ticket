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

        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (loginDto == null)
            {
                return new BadRequestObjectResult("Login data insufficient");
            }

            var _user = _context.Users.Where(e=> e.Email == loginDto.Email).FirstOrDefault();

            if (_user == null)
            {
                return new NotFoundObjectResult("User Not Found");
            }
            // Verify the password
            if (!PasswordHasher.VerifyPasswordHash(loginDto.Password, _user.PasswordHash, _user.PasswordSalt))
                return new UnauthorizedObjectResult(new { message = "Invalid email or password."});


            return new OkObjectResult(new {message = "Loggedin successfully"});
        }
    }
}
