using PeerToPeerCall.Attributes;
using System.ComponentModel.DataAnnotations;

namespace PeerToPeerCall.Models
{
    public class UserSignUpDto
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Username is required")]
        [StringLength(50, ErrorMessage = "Username must be beetwen 5 and 50 characters", MinimumLength = 5)]
        [OnlyAllowedCharacters("AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz123456789!@#$%^&*(){}[]", ErrorMessage = "Username contains not allowed characters")]
        public string UserName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "E-mail is required")]
        [EmailAddress(ErrorMessage = "E-mail is invalid")]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required")]
        [StringLength(255, ErrorMessage = "Password must be beetwen 6 and 255 characeters", MinimumLength = 6)]
        [RequiredDigit(1, ErrorMessage = "Password must contain at least one number")]
        [RequiredLowerCase(1, ErrorMessage = "Password must contain at least one lowercase letter")]
        [RequiredUpperCase(1, ErrorMessage = "Password must contain at least one uppercase letter")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Passwords do not matches")]
        public string RepeatPassword { get; set; }
    }
}
