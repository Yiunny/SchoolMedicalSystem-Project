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
    public class CheckupPlanRepository : ICheckupPlanRepository
    {
        private readonly SchoolDbContext _context;

        public CheckupPlanRepository(SchoolDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(CheckupPlan plan)
        {
            await _context.CheckupPlans.AddAsync(plan);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var plan = await _context.CheckupPlans.FindAsync(id);
            if (plan != null)
            {
                _context.CheckupPlans.Remove(plan);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<CheckupPlan>> GetAllAsync()
        {
            return await _context.CheckupPlans.OrderByDescending(p => p.CheckupDate).ToListAsync();
        }

        public async Task<CheckupPlan> GetByIdAsync(int id)
        {
            return await _context.CheckupPlans.FindAsync(id);
        }

        public async Task UpdateAsync(CheckupPlan plan)
        {
            var existingPlan = await _context.CheckupPlans.FindAsync(plan.Id);

            if (existingPlan != null)
            {
                existingPlan.PlanName = plan.PlanName;
                existingPlan.ClassName = plan.ClassName;
                existingPlan.CheckupDate = plan.CheckupDate;
                existingPlan.Status = plan.Status;

                await _context.SaveChangesAsync();
            }
        }
    }
}
