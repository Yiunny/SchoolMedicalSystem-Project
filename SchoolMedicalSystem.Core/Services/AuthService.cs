using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SchoolMedicalSystem.Core.Models;
using SchoolMedicalSystem.Core.Repositories;

namespace SchoolMedicalSystem.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;

        // Service sẽ phụ thuộc vào IUserRepository, không phải DbContext
        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<LoginResult> LoginAsync(string username, string password)
        {
            var user = await _userRepository.GetByUsernameAsync(username);

            // Kiểm tra user không tồn tại hoặc mật khẩu sai
            // LƯU Ý: Đây là phần kiểm tra mật khẩu đơn giản. Trong thực tế, bạn cần dùng
            // một thư viện như BCrypt.Net để so sánh password với PasswordHash.
            // Tạm thời chúng ta sẽ giả sử mật khẩu đúng để đi tiếp.
            if (user == null || user.PasswordHash != password) // Tạm thời so sánh trực tiếp
            {
                return new LoginResult { IsSuccess = false, ErrorMessage = "Tên đăng nhập hoặc mật khẩu không chính xác." };
            }

            // Kiểm tra vai trò của người dùng
            switch (user.Role)
            {
                case UserRole.Manager:
                case UserRole.Nurse:
                    // Vai trò hợp lệ, cho phép đăng nhập
                    return new LoginResult { IsSuccess = true, User = user };

                case UserRole.Admin:
                case UserRole.Parent:
                case UserRole.Student:
                    // Vai trò không được phép truy cập
                    return new LoginResult { IsSuccess = false, ErrorMessage = "Bạn không được phép truy cập hệ thống." };

                default:
                    // Các trường hợp khác
                    return new LoginResult { IsSuccess = false, ErrorMessage = "Vai trò người dùng không hợp lệ." };
            }
        }
    }
}
