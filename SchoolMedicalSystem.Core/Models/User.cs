using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolMedicalSystem.Core.Models
{
    public enum UserRole
    {
        // 2 vai trò được triển khai
        Manager,
        Nurse,

        // 3 vai trò cho tương lai
        Admin,
        Parent,
        Student
    }

    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public UserRole Role { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
