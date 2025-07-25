using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SchoolMedicalSystem.Core.Models;
using SchoolMedicalSystem.Core.Repositories;

namespace SchoolMedicalSystem.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<User> GetManagerAccountAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.FirstOrDefault(u => u.Role == UserRole.Manager);
        }

        public async Task<User> GetNurseAccountAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.FirstOrDefault(u => u.Role == UserRole.Nurse);
        }

        public async Task UpdateUserAsync(User user)
        { 
            await _userRepository.UpdateAsync(user);
        }
    }
}
