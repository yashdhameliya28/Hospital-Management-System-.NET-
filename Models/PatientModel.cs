﻿using Microsoft.AspNetCore.Mvc;

namespace HMS.Models
{
    public class PatientModel : Controller
    {
       public int PatientID { get; set; }
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public bool IsActive  { get; set; }
        public int UserID   { get; set; }
    }
}
