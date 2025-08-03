using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace HMS.Models
{
    public class DoctorModel
    {
        public int DoctorID { get; set; }
        
        [Required(ErrorMessage ="Required Name")]
        [StringLength(maximumLength:20, ErrorMessage ="Not valid name", MinimumLength = 2)]
        public string Name { get; set; }

        [Required(ErrorMessage ="Required Phone Number")]
        [Phone(ErrorMessage ="Not valid Number")]
        public string Phone { get; set; }

        [Required(ErrorMessage ="Required Email Address")]
        [EmailAddress(ErrorMessage ="Enter valid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Required Qualification")]
        [StringLength(100, MinimumLength =4, ErrorMessage = "Enter valid Qualification")]
        public string Qualification { get; set; }

        [Required(ErrorMessage = "Required Specialization")]
        [StringLength(10, MinimumLength = 5, ErrorMessage = "Enter valid Specialization")]
        public string Specialization { get; set; }
       
        public bool IsActive { get; set; }
        public DateTime Modified { get; set; } = DateTime.Now;
        public DateTime Created { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Required UserID")]
        public int UserID { get; set; }

    }
}
