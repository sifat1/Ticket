using System.ComponentModel.DataAnnotations;

namespace ShowTickets.Ticketmodels.User{
    public class User
    {
        public long UserId { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Password { get; set; }
        public List<Role>? Roles { get; set; }

    }

}