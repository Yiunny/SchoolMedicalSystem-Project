using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolMedicalSystem.Core.Models
{
    public enum Gender { Nam, Nữ, Khác }

    public class Student
    {
        public int Id { get; set; }
        public string StudentCode { get; set; }
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public string Address { get; set; }
        public string ParentName { get; set; }
        public string ParentPhoneNumber { get; set; }
        public string? Allergies { get; set; } // Dấu ? cho biết trường này có thể null
        public string ClassName { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
