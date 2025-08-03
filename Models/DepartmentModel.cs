using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace HMS.Models
{
    public class DepartmentModel 
    {
        public int DepartmentID { get; set; }

        [Required(ErrorMessage = "Required Name")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Enter valid Name")]
        public string DepartmentName { get; set; }

        public bool IsActive { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        [Required(ErrorMessage = "Required Description")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Enter valid Description")]
        public string Description { get; set; }

        [Required(ErrorMessage ="Required UserID")]
        public int UserID { get; set; }
    }
}
