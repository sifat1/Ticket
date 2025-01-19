using System.ComponentModel.DataAnnotations;

namespace Dtos
{

    public class RegistrationDTO
    {
        [Required]
        [StringLength(20)]
        //[RegularExpression(@"^[^\s]$")]
        public string Name { get; set; }
        [Required]
        [StringLength(30)]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
        public string Email { get; set; }
        [Required]
        [RegularExpression(@"^\d{11}$")]
        public string Phone { get; set; }
        [Required]
        [RegularExpression(@"^[A-z|0-9|@$!%*#?&]{8,}$")]
        [StringLength(15, ErrorMessage = "Passowrd must be atleast 8 chacater long", MinimumLength = 8)]
        public string Password { get; set; }
    }
}
