using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace HMS.Models
{
    public class UserLoginModel 
    {
        [Required(ErrorMessage ="Required User Name.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Required Password.")]
        public string Password { get; set; }
    }
}
