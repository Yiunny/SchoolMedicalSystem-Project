using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SchoolMedicalSystem.Core.Models;

namespace SchoolMedicalSystem.Core.Repositories
{
    public interface ICheckupPlanRepository
    {
        Task<CheckupPlan> GetByIdAsync(int id);
        Task<IEnumerable<CheckupPlan>> GetAllAsync();
        Task AddAsync(CheckupPlan plan);
        Task UpdateAsync(CheckupPlan plan);
        Task DeleteAsync(int id);
    }
}
