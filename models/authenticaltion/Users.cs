using System.ComponentModel.DataAnnotations;

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
        public ICollection<UserRole> UserRoles { get; set; } // Navigation property for many-to-many relationship with Role

    }

}