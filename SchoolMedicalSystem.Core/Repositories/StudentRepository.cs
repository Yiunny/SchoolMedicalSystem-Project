using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SchoolMedicalSystem.Core.Data;
using SchoolMedicalSystem.Core.Models;

namespace SchoolMedicalSystem.Core.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly SchoolDbContext _context;

        public StudentRepository(SchoolDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Student student)
        {
            await _context.Students.AddAsync(student);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                student.IsActive = false; 
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Student>> GetAllAsync()
        {

            return await _context.Students.Where(s => s.IsActive).ToListAsync();
        }

        public async Task<Student> GetByIdAsync(int id)
        {
            return await _context.Students.FindAsync(id);
        }

        public async Task UpdateAsync(Student student)
        {
            var existingStudent = await _context.Students.FindAsync(student.Id);

            if (existingStudent != null)
            {
                existingStudent.FullName = student.FullName;
                existingStudent.StudentCode = student.StudentCode;
                existingStudent.DateOfBirth = student.DateOfBirth;
                existingStudent.Gender = student.Gender;
                existingStudent.Address = student.Address;
                existingStudent.ParentName = student.ParentName;
                existingStudent.ParentPhoneNumber = student.ParentPhoneNumber;
                existingStudent.Allergies = student.Allergies;
                existingStudent.ClassName = student.ClassName;
                existingStudent.IsActive = student.IsActive;

                await _context.SaveChangesAsync();
            }
        }
    }
}
