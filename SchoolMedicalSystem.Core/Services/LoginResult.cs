using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SchoolMedicalSystem.Core.Models;

namespace SchoolMedicalSystem.Core.Services
{
    public class LoginResult
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public UserRole? UserRole { get; set; }
    }
}
