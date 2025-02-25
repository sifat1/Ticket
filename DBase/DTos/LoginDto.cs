using System.ComponentModel.DataAnnotations;

namespace Dtos
{
    public class LoginDto
    {
        [Required]
        [StringLength(100)]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"^[A-Za-z0-9@$!%*#?&]{8,}$")]
        [StringLength(15, ErrorMessage = "Passowrd must be atleast 8 chacater long", MinimumLength = 8)]
        public string Password { get; set; }
    }
}