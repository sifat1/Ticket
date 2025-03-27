using System.ComponentModel.DataAnnotations;
using JWTAuthServer.Models;

namespace ShowTickets.Ticketmodels.User
{
    public class Users
    {
        [Key]
        public long UserId { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public byte[] PasswordHash { get; set; }
        [Required]
        public byte[] PasswordSalt { get; set; }
        
        public string Role { get; set; } = "User";  // Default role is "User"
        public List<RefreshToken> RefreshTokens { get; set; } = new();

        public List<ShowSeat> ShowTickets { get; set; } = new();

    }

}