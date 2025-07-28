using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SchoolMedicalSystem.Core.Models;

namespace SchoolMedicalSystem.Core.Services
{
    public interface IStudentService
    {
        Task<IEnumerable<Student>> GetAllStudentsAsync();
        Task AddStudentAsync(Student student);
        Task UpdateStudentAsync(Student student);
        Task DeleteStudentAsync(int studentId);
        Task<IEnumerable<Student>> GetStudentsByClassNameAsync(string className);
        Task<IEnumerable<Student>> SearchStudentsAsync(string searchTerm);
        Task<string> ImportStudentsFromExcelAsync(string filePath);  
    }
}
