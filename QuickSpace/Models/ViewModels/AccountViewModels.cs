using System.ComponentModel.DataAnnotations;

namespace QuickSpace.Models.ViewModels
{
    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
        public string ReturnUrl { get; internal set; }
    }
    public class ForgotPasswordViewModel {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address provided")]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
    public class VerificationViewModel { 
        [Required, Display(Name ="Verification Code")]
        public string VerificationCode { get; set; }
    }
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Please provide your Full Name")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address provided")]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required]
        [Phone(ErrorMessage = "Invalid phone number provided.")]
        [Display(Name = "Phone Number")]
        public string Number { get; set; }

        [Display(Name = "Country")]
        public string Country { get; set; }
        [Display(Name = "Referral Code (Leave as '0000' if no one Invited you)")]
        public string ParentId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Consent Terms and Conditions")]
        public bool Consent { get; set; }
    }
}
