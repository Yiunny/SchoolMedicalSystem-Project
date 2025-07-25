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
    public class HealthRecordRepository : IHealthRecordRepository
    {
        private readonly SchoolDbContext _context;

        public HealthRecordRepository(SchoolDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(HealthRecord record)
        {
            await _context.HealthRecords.AddAsync(record);
            await _context.SaveChangesAsync();
        }

        public async Task<HealthRecord> GetByPlanAndStudentIdAsync(int planId, int studentId)
        {
            return await _context.HealthRecords
                .FirstOrDefaultAsync(hr => hr.CheckupPlanId == planId && hr.StudentId == studentId);
        }

        public async Task UpdateAsync(HealthRecord record)
        {
            _context.HealthRecords.Update(record);
            await _context.SaveChangesAsync();
        }
    }
}
