using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace HMS.Models
{
    public class DoctorDepartmentModel
    {

        public int DoctorDepartmentID { get; set; }

        [Required(ErrorMessage ="Required Doctor Name")]
        public string DoctorName { get; set; }

        [Required(ErrorMessage = "Required Department Name")]
        public string DepartmentName { get; set; }

        [Required(ErrorMessage = "Required DoctorID Name")]
        public int DoctorID { get; set; }

        [Required(ErrorMessage = "Required DepartmentID Name")]
        public int DepartmentID { get; set; }


        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        [Required(ErrorMessage = "Required UserID Name")]
        public int UserID { get; set; }
    }

    public class User_DropDownModel
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
    }

    public class Doctor_DropDownModel
    {
        public int DoctorID { get; set; }
        public string DoctorName { get; set; }
    }   

    public class Department_DropDown
    {
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; }
    }
}
