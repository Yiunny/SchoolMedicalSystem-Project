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
            CalculateBmi(record);
            await _healthRecordRepository.AddAsync(record);
        }

        public async Task<HealthRecord> GetByPlanAndStudentIdAsync(int planId, int studentId)
        {
            return await _healthRecordRepository.GetByPlanAndStudentIdAsync(planId, studentId);
        }

        public async Task UpdateHealthRecordAsync(HealthRecord record)
        {
            CalculateBmi(record);
            await _healthRecordRepository.UpdateAsync(record);
        }
        public async Task<IEnumerable<HealthRecord>> GetHistoryByStudentIdAsync(int studentId)
        {
            return await _healthRecordRepository.GetByStudentIdAsync(studentId);
        }

        private void CalculateBmi(HealthRecord record)
        {
            if (record.Height <= 0)
            {
                record.Bmi = 0;
                record.BmiStatus = "Không xác định";
                return;
            }

            // Chuyển đổi chiều cao từ cm sang mét
            double heightInMeters = record.Height / 100.0;

            // Tính BMI
            double bmi = record.Weight / (heightInMeters * heightInMeters);
            record.Bmi = Math.Round(bmi, 2); 

            if (bmi < 18.5)
            {
                record.BmiStatus = "Thiếu cân";
            }
            else if (bmi < 24.9)
            {
                record.BmiStatus = "Bình thường";
            }
            else if (bmi < 29.9)
            {
                record.BmiStatus = "Thừa cân";
            }
            else
            {
                record.BmiStatus = "Béo phì";
            }
        }

    }
}
