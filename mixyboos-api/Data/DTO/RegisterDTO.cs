using System.ComponentModel.DataAnnotations;

namespace MixyBoos.Api.Data.DTO {
    public class RegisterDTO {
        [Required]
        [EmailAddress]
        [Display(Name = "UserName")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Password and Confirm Password must match")]
        public string ConfirmPassword { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "Profile name cannot be longer than 30 characters")]
        [Display(Name = "DisplayName")]
        public string DisplayName { get; set; }
    }
}
