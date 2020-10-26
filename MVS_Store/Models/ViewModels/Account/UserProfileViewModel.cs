using MVS_Store.Models.Data;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MVS_Store.Models.ViewModels.Account
{
    public class UserProfileViewModel
    {
        public UserProfileViewModel()
        {
        }

        public UserProfileViewModel(UserDTO row)
        {
            ID = row.ID;
            FirstName = row.FirstName;
            LastName = row.LastName;
            EmailAddress = row.EmailAddress;
            UserName = row.UserName;
            Password = row.Password;
        }

        public int ID { get; set; }
        [Required]
        [DisplayName("First Name")]
        public string FirstName { get; set; }
        [Required]
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [DisplayName("Email")]
        public string EmailAddress { get; set; }
        [Required]
        [DisplayName("User Name")]
        public string UserName { get; set; }
        public string Password { get; set; }
        [DisplayName("Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}