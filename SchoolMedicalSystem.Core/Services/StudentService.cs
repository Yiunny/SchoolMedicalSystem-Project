using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
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
            string yearPrefix = student.DateOfBirth.ToString("yy");
            var allStudents = await _studentRepository.GetAllAsync();
            var maxSequence = allStudents
                .Where(s => s.StudentCode.StartsWith(yearPrefix))
                .Select(s =>
                {
                    int.TryParse(s.StudentCode.Substring(2), out int sequence);
                    return sequence;
                })
                .DefaultIfEmpty(0)
                .Max();

            int newSequence = maxSequence + 1;
            student.StudentCode = $"{yearPrefix}{newSequence:D3}";
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

        public async Task<string> ImportStudentsFromExcelAsync(string filePath)
        {
            var newStudents = new List<Student>();

            // Correct way to set the license for EPPlus
            ExcelPackage.License.SetNonCommercialPersonal("School Medical System Project");

            try
            {
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    if (worksheet == null)
                    {
                        return "File Excel không hợp lệ hoặc không có worksheet nào.";
                    }

                    int rowCount = worksheet.Dimension.Rows;
                    for (int row = 2; row <= rowCount; row++)
                    {
                        try
                        {
                            var student = new Student
                            {
                                FullName = worksheet.Cells[row, 1].GetValue<string>(),
                                DateOfBirth = worksheet.Cells[row, 2].GetValue<DateTime>(),
                                Gender = worksheet.Cells[row, 3].GetValue<string>() == "Nam" ? Gender.Nam : Gender.Nữ,
                                ClassName = worksheet.Cells[row, 4].GetValue<string>(),
                                Address = worksheet.Cells[row, 5].GetValue<string>(),
                                ParentName = worksheet.Cells[row, 6].GetValue<string>(),
                                ParentPhoneNumber = worksheet.Cells[row, 7].GetValue<string>(),
                                Allergies = worksheet.Cells[row, 8].GetValue<string>(),
                                IsActive = true
                            };
                            newStudents.Add(student);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Lỗi ở dòng {row}: {ex.Message}");
                        }
                    }
                }

                foreach (var student in newStudents)
                {
                    await AddStudentAsync(student);
                }

                return $"Import thành công {newStudents.Count} học sinh.";
            }
            catch (Exception ex)
            {
                return $"Đã xảy ra lỗi khi đọc file: {ex.Message}";
            }
        }
    }
}