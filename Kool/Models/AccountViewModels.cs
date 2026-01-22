using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Kool.Models
{
    // ====== VÄLINE SISSELOGIMINE ======

    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "E-post")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    // ====== 2FA / KOOD ======

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }

        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Kood")]
        public string Code { get; set; }

        public string ReturnUrl { get; set; }

        [Display(Name = "Jäta brauser meelde")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    // ====== SISSELOGIMINE ======

    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "E-post")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Parool")]
        public string Password { get; set; }

        [Display(Name = "Jäta mind meelde")]
        public bool RememberMe { get; set; }
    }

    // ====== REGISTREERIMINE (ÕPILANE) ======

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "E-post")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Parool peab olema vähemalt {2} tähemärki.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Parool")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Kinnita parool")]
        [Compare("Password", ErrorMessage = "Paroolid ei kattu.")]
        public string ConfirmPassword { get; set; }
    }

    // ====== PAROOLI TAASTAMINE ======

    public class ForgotViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "E-post")]
        public string Email { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "E-post")]
        public string Email { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "E-post")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Parool peab olema vähemalt {2} tähemärki.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Uus parool")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Kinnita parool")]
        [Compare("Password", ErrorMessage = "Paroolid ei kattu.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }
}
