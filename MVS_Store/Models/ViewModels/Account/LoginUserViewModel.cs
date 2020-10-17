using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MVS_Store.Models.ViewModels.Account
{
    public class LoginUserViewModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [DisplayName("Remember Me")]
        public bool RememberMe { get; set; }
    }
}