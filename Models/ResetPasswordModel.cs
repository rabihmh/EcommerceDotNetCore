using System.ComponentModel.DataAnnotations;

namespace EcommerceDotNetCore.Models
{
    public class ResetPasswordModel
    {
        [Required]
        public string Token { get; set; }

        [Required]
        [EmailAddress] 
        public string Email { get; set; }

        [Required]
        [StringLength(50,MinimumLength =8)]
        public string NewPassword { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 8)]
        [Compare("NewPassword",ErrorMessage ="Password doesn't match its confirmation")]
        public string ConfirmPassword { get; set;}

    }
}
