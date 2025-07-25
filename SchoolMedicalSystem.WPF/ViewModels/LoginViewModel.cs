using Microsoft.Extensions.DependencyInjection; // Thêm để dùng IServiceProvider
using SchoolMedicalSystem.Core.Services;
using SchoolMedicalSystem.WPF.Views;          // Thêm để biết LoginWindow và MainWindow
using System;                                 // Thêm để dùng IServiceProvider
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SchoolMedicalSystem.WPF.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        // --- THAY ĐỔI 1: Thêm IServiceProvider ---
        private readonly IAuthService _authService;
        private readonly IServiceProvider _serviceProvider;
        private string _username;
        private string _errorMessage;

        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; }

        // --- THAY ĐỔI 2: Cập nhật Constructor để nhận thêm IServiceProvider ---
        public LoginViewModel(IAuthService authService, IServiceProvider serviceProvider)
        {
            _authService = authService;
            _serviceProvider = serviceProvider; // Lưu lại service provider
            LoginCommand = new RelayCommand(async (param) => await Login(param), (param) => CanLogin());
        }

        private bool CanLogin()
        {
            return !string.IsNullOrEmpty(Username);
        }

        // --- THAY ĐỔI 3: Cập nhật hoàn toàn phương thức Login ---
        private async Task Login(object parameter)
        {
            var passwordBox = parameter as PasswordBox;
            if (passwordBox == null) return;
            var password = passwordBox.Password;

            ErrorMessage = string.Empty; // Xóa thông báo lỗi cũ

            var result = await _authService.LoginAsync(Username, password);

            if (result.IsSuccess)
            {
                var mainViewModel = _serviceProvider.GetService<MainViewModel>();
                // Khởi tạo MainViewModel với vai trò của người dùng
                mainViewModel.Initialize(result.UserRole.Value);

                // Lấy MainWindow và gán DataContext của nó là MainViewModel
                var mainWindow = _serviceProvider.GetService<MainWindow>();
                mainWindow.DataContext = mainViewModel;

                mainWindow.Show();

                // Tìm và đóng cửa sổ đăng nhập hiện tại
                foreach (Window window in Application.Current.Windows)
                {
                    if (window is LoginWindow)
                    {
                        window.Close();
                        break;
                    }
                }
            }
            else
            {
                ErrorMessage = result.ErrorMessage;
            }
        }

        // Boilerplate code for INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
     
}