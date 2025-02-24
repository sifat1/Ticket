using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using DB.DBcontext;
using Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShowTickets.Ticketmodels.User;
using User.Password;

namespace User.Registration
{
    public class UserRegistrationService
    {
        private readonly ShowDbContext _context;
        private readonly IConfiguration _configuration;

        public UserRegistrationService(ShowDbContext dbContext,IConfiguration configuration)
        {
            _context = dbContext;
            _configuration = configuration;
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

            var _user = _context.Users.Where(e => e.Email == loginDto.Email).FirstOrDefault();

            if (_user == null)
            {
                return new NotFoundObjectResult("User Not Found");
            }
            // Verify the password
            if (!PasswordHasher.VerifyPasswordHash(loginDto.Password, _user.PasswordHash, _user.PasswordSalt))
                return new UnauthorizedObjectResult(new { message = "Invalid email or password." });

            var token = GenerateJwtToken(_user);
            return new OkObjectResult(new { message = token });
        }

        // Private method responsible for generating a JWT token for an authenticated user
        private string GenerateJwtToken(Users user)
        {
            // Retrieve the active signing key from the SigningKeys table
            var signingKey = _context.SigningKeys.FirstOrDefault(k => k.IsActive);

            // If no active signing key is found, throw an exception
            if (signingKey == null)
            {
                throw new Exception("No active signing key available.");
            }

            // Convert the Base64-encoded private key string back to a byte array
            var privateKeyBytes = Convert.FromBase64String(signingKey.PrivateKey);

            // Create a new RSA instance for cryptographic operations
            var rsa = RSA.Create();

            // Import the RSA private key into the RSA instance
            rsa.ImportRSAPrivateKey(privateKeyBytes, out _);

            // Create a new RsaSecurityKey using the RSA instance
            var rsaSecurityKey = new RsaSecurityKey(rsa)
            {
                // Assign the Key ID to link the JWT with the correct public key
                KeyId = signingKey.KeyId
            };

            // Define the signing credentials using the RSA security key and specifying the algorithm
            var creds = new SigningCredentials(rsaSecurityKey, SecurityAlgorithms.RsaSha256);

            // Initialize a list of claims to include in the JWT
            var claims = new List<Claim>
            {
                // Subject (sub) claim with the user's ID
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),

                // JWT ID (jti) claim with a unique identifier for the token
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

                // Name claim with the user's first name
                new Claim(ClaimTypes.Name, user.Name),

                // NameIdentifier claim with the user's email
                new Claim(ClaimTypes.NameIdentifier, user.Email),

                // Email claim with the user's email
                new Claim(ClaimTypes.Email, user.Email)
            };

            // Iterate through the user's roles and add each as a Role claim
            foreach (var userRole in user.UserRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));
            }

            // Define the JWT token's properties, including issuer, audience, claims, expiration, and signing credentials
            var tokenDescriptor = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"], // The token issuer, typically your application's URL
                audience: "webapp", // The intended recipient of the token, typically the client's URL
                claims: claims, // The list of claims to include in the token
                expires: DateTime.UtcNow.AddHours(1), // Token expiration time set to 1 hour from now
                signingCredentials: creds // The credentials used to sign the token
            );

            // Create a JWT token handler to serialize the token
            var tokenHandler = new JwtSecurityTokenHandler();

            // Serialize the token to a string
            var token = tokenHandler.WriteToken(tokenDescriptor);

            // Return the serialized JWT token
            return token;
        }
    }
}
