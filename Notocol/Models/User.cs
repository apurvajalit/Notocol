using System.ComponentModel.DataAnnotations;

namespace Notocol.Models
{
    public class User
    {
        [Display(Name = "User name")]
        public string UserName { get; set; }


        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}