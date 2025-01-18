using System.ComponentModel.DataAnnotations;

namespace ShowTickets.Ticketmodels.User
{
    public class Users
    {
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
        public List<Role>? Roles { get; set; }

    }

}