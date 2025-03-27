using System.ComponentModel.DataAnnotations;

namespace ShowTickets.Ticketmodels.User
{
    public class Role
    {
        // Primary key for the Role entity.
        [Key]
        public int Id { get; set; }
        // Name of the role (e.g., Admin, User).
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        //Role Description
        public string? Description { get; set; }
        
    }
}