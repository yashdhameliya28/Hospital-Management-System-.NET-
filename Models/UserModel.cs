using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace HMS.Models
{
    public class UserModel
    {
        public int UserID { get; set; }

        [Required(ErrorMessage = "Required Name")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Enter valid Name")]
        public string UserName { get; set; }

        [Required(ErrorMessage ="required Password")]
        [StringLength(20, MinimumLength =6, ErrorMessage ="Enter valid password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Required Email Adress")]
        [EmailAddress(ErrorMessage = "Enter valid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Required Phone Number")]
        [Phone(ErrorMessage = "Enter valid Phone Number")]
        public string MobileNo { get; set; }

        public bool IsActive { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }
}
