using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace HMS.Models
{
    public class PatientModel
    {
        public int PatientID { get; set; }

        [Required(ErrorMessage = "Required Name")]
        [StringLength(50, MinimumLength =2, ErrorMessage ="Enter valid Name")]
        public string PatientName { get; set; }

        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage ="Required Gender")]
        public string Gender { get; set; }

        [Required(ErrorMessage ="Required Email Adress")]
        [EmailAddress(ErrorMessage = "Enter valid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage ="Required Phone Number")]
        [Phone(ErrorMessage ="Enter valid Phone Number")]
        public string Phone { get; set; }

        [Required(ErrorMessage ="Requird Adddress")]
        [StringLength(100, MinimumLength =4, ErrorMessage ="Enter Valid Address")]
        public string Address { get; set; }

        [Required(ErrorMessage ="Required City")]
        [StringLength(20, MinimumLength =4, ErrorMessage ="Enter Valid City")]
        public string City { get; set; }

        [Required(ErrorMessage ="Required State")]
        public string State { get; set; }

        public bool IsActive { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        [Required(ErrorMessage ="Required User name")]
        public int UserID { get; set; }

        [Required(ErrorMessage ="Required User Name")]
        public int UserName { get; set; }
    }

}
