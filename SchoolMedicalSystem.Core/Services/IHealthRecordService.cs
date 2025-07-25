using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SchoolMedicalSystem.Core.Models;

namespace SchoolMedicalSystem.Core.Services
{
    public interface IHealthRecordService
    {
        Task CreateHealthRecordAsync(HealthRecord record);
        Task UpdateHealthRecordAsync(HealthRecord record); 
        Task<HealthRecord> GetByPlanAndStudentIdAsync(int planId, int studentId); 
    }
}
