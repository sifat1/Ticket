using System.ComponentModel.DataAnnotations;
using ShowTickets.Ticketmodels.User;

namespace JWTAuthServer.Models;

public class RefreshToken
{
    public int Id { get; set; }
    public string Token { get; set; }
    public DateTime Expires { get; set; }
    public bool IsRevoked { get; set; }

    // Foreign key to the user
    public int UserId { get; set; }
    public Users User { get; set; }
}
