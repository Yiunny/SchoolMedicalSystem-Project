using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SchoolMedicalSystem.Core.Models;
using SchoolMedicalSystem.Core.Repositories;

namespace SchoolMedicalSystem.Core.Services
{
    public class CheckupPlanService : ICheckupPlanService
    {
        private readonly ICheckupPlanRepository _checkupPlanRepository;

        public CheckupPlanService(ICheckupPlanRepository checkupPlanRepository)
        {
            _checkupPlanRepository = checkupPlanRepository;
        }

        public async Task AddPlanAsync(CheckupPlan plan)
        {
            // Có thể thêm logic kiểm tra nghiệp vụ ở đây trước khi lưu
            plan.Status = CheckupPlanStatus.Planned; // Mặc định trạng thái khi mới tạo
            await _checkupPlanRepository.AddAsync(plan);
        }

        public async Task<IEnumerable<CheckupPlan>> GetAllPlansAsync()
        {
            return await _checkupPlanRepository.GetAllAsync();
        }

        public async Task UpdatePlanAsync(CheckupPlan plan)
        {
            await _checkupPlanRepository.UpdateAsync(plan);
        }

        public async Task UpdatePlanStatusAsync(int planId, CheckupPlanStatus newStatus)
        {
            var plan = await _checkupPlanRepository.GetByIdAsync(planId);
            if (plan != null)
            {
                plan.Status = newStatus;
                await _checkupPlanRepository.UpdateAsync(plan);
            }
        }
    }
}
