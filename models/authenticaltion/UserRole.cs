namespace ShowTickets.Ticketmodels.User
{
    public class UserRole
    {
        // Foreign key referencing User.
        public int UserId { get; set; }

        // Navigation property to User.
        public Users User { get; set; }

        // Foreign key referencing Role.
        public int RoleId { get; set; }

        // Navigation property to Role.
        public Role Role { get; set; }
    }
}