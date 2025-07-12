using Microsoft.AspNetCore.Mvc;

namespace HMS.Models
{
    public class DoctorDepartmentModel : Controller
    {
        public int DoctorDepartmentID { get; set; }
        public string DoctorName { get; set; }
        public string DepartmentName { get; set; }
        public int DoctorID { get; set; }
        public int DepartmentID { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public int UserID { get; set; }
    }
}
