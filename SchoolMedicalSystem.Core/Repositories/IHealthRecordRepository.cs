using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SchoolMedicalSystem.Core.Models;

namespace SchoolMedicalSystem.Core.Repositories
{
    public interface IHealthRecordRepository
    {
        Task AddAsync(HealthRecord record);
        Task UpdateAsync(HealthRecord record); 
        Task<HealthRecord> GetByPlanAndStudentIdAsync(int planId, int studentId);  
    }
}
