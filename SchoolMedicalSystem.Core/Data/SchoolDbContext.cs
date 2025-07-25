using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SchoolMedicalSystem.Core.Models;

namespace SchoolMedicalSystem.Core.Data
{
    public class SchoolDbContext : DbContext
    {
        // Khai báo các bảng trong database
        public DbSet<Student> Students { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<HealthRecord> HealthRecords { get; set; }

        public DbSet<CheckupPlan> CheckupPlans { get; set; }

        // Cấu hình để sử dụng SQLite
        // File: SchoolDbContext.cs

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Xóa hoặc ghi chú dòng UseSqlite cũ
            // optionsBuilder.UseSqlite("Data Source=school_medical.db");

            // Thêm dòng UseSqlServer mới
            optionsBuilder.UseSqlServer(
                "Server=LAPTOP-GQFO67BC\\NDC;Database=SchoolMedicalDB;Trusted_Connection=True;TrustServerCertificate=True"
            );
        }   
    }
}
