using System.ComponentModel.DataAnnotations;

namespace Roham.Web.ViewModels
{
    public class ExternalLoginListViewModel
    {
        public string Action { get; set; }
        public string ReturnUrl { get; set; }
    }

    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        public string RegisterText => "Please enter a user name for this site below and click the Register button to finish logging in.";
        public string AssociateText => $"Please confirm to associate {Email} with external provider";
    }
}
