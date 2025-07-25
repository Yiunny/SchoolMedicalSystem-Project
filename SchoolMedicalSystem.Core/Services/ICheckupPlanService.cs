using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SchoolMedicalSystem.Core.Models;

namespace SchoolMedicalSystem.Core.Services
{
    public interface ICheckupPlanService
    {
        Task<IEnumerable<CheckupPlan>> GetAllPlansAsync();
        Task AddPlanAsync(CheckupPlan plan);
        Task UpdatePlanAsync(CheckupPlan plan);
        Task UpdatePlanStatusAsync(int planId, CheckupPlanStatus newStatus);  
    }
}
