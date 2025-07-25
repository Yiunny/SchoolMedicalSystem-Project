using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SchoolMedicalSystem.Core.Models;
using SchoolMedicalSystem.Core.Repositories;

namespace SchoolMedicalSystem.Core.Services
{
    public class HealthRecordService : IHealthRecordService
    {
        private readonly IHealthRecordRepository _healthRecordRepository;

        public HealthRecordService(IHealthRecordRepository healthRecordRepository)
        {
            _healthRecordRepository = healthRecordRepository;
        }

        public async Task CreateHealthRecordAsync(HealthRecord record)
        { 
            await _healthRecordRepository.AddAsync(record);
        }

        public async Task<HealthRecord> GetByPlanAndStudentIdAsync(int planId, int studentId)
        {
            return await _healthRecordRepository.GetByPlanAndStudentIdAsync(planId, studentId);
        }

        public async Task UpdateHealthRecordAsync(HealthRecord record)
        {
            await _healthRecordRepository.UpdateAsync(record);
        }
    }
}
