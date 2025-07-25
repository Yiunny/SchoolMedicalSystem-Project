using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SchoolMedicalSystem.Core.Models;

namespace SchoolMedicalSystem.Core.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GetManagerAccountAsync();
        Task<User> GetNurseAccountAsync();
        Task UpdateUserAsync(User user);
    }
}
