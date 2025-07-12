using Microsoft.AspNetCore.Mvc;

namespace HMS.Models
{
    public class AppointmentModel : Controller
    {
        public int AppointmentID { get; set; }
        public int DoctorID { get; set; }
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public string DoctorName { get; set; }
        public string Specialization { get; set; }
        public string UserName { get; set; }
        public int PatientID    { get; set; }
        public string PatientName { get; set; }
        public string PatientEmail { get; set; }    
        public DateTime AppointmentDate { get; set; }
        public string AppointmentStatus { get; set; }
        public string Description { get; set; }
        public string SpecialRemarks { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public int UserID { get; set; }
        public double TotalConsultedAmount { get; set; }
    }
}
