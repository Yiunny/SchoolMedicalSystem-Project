using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SchoolMedicalSystem.Core.Models;
using SchoolMedicalSystem.Core.Repositories;

namespace SchoolMedicalSystem.Core.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;

        public StudentService(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public async Task AddStudentAsync(Student student)
        {
            // 1. Lấy tiền tố từ năm sinh (ví dụ: 2017 -> "17")
            string yearPrefix = student.DateOfBirth.ToString("yy");

            // 2. Lấy tất cả học sinh để tìm mã lớn nhất trong cùng năm
            var allStudents = await _studentRepository.GetAllAsync();

            // 3. Tìm số thứ tự lớn nhất của các học sinh có cùng năm sinh
            var maxSequence = allStudents
                .Where(s => s.StudentCode.StartsWith(yearPrefix)) // Lọc các mã bắt đầu bằng "17"
                .Select(s => {
                    // Thử chuyển đổi 3 số cuối thành số nguyên
                    int.TryParse(s.StudentCode.Substring(2), out int sequence);
                    return sequence;
                })
                .DefaultIfEmpty(0) // Nếu chưa có ai trong năm, bắt đầu từ 0
                .Max();

            // 4. Tạo mã mới
            int newSequence = maxSequence + 1;
            student.StudentCode = $"{yearPrefix}{newSequence:D3}"; // D3 để đảm bảo có 3 chữ số (ví dụ: 001, 002)

            // 5. Lưu học sinh với mã mới vào database
            await _studentRepository.AddAsync(student);
        }

        public async Task DeleteStudentAsync(int studentId)
        {
            await _studentRepository.DeleteAsync(studentId);
        }

        public async Task<IEnumerable<Student>> GetAllStudentsAsync()
        {
            return await _studentRepository.GetAllAsync();
        }

        public async Task UpdateStudentAsync(Student student)
        {
            await _studentRepository.UpdateAsync(student);
        }

        public async Task<IEnumerable<Student>> GetStudentsByClassNameAsync(string className)
        {
            var allStudents = await _studentRepository.GetAllAsync();
            return allStudents.Where(s => s.ClassName == className);
        }

        public async Task<IEnumerable<Student>> SearchStudentsAsync(string searchTerm)
        {
            var allStudents = await _studentRepository.GetAllAsync();
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return allStudents;
            }

            return allStudents.Where(s =>
                s.FullName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                s.StudentCode.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            );
        }
    }
}
