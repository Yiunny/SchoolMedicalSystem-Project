using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Input;
using SchoolMedicalSystem.Core.Models;
using SchoolMedicalSystem.WPF.Views;
using System.Windows;

namespace SchoolMedicalSystem.WPF.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {

        private readonly IServiceProvider _serviceProvider;
        private object _currentView;
        private User _currentUser;
        public object CurrentView
        {
            get => _currentView;
            set { _currentView = value; OnPropertyChanged(); }
        }
        private UserRole _currentUserRole;
        public UserRole CurrentUserRole
        {
            get => _currentUserRole;
            set { _currentUserRole = value; OnPropertyChanged(); }
        }

        // Có thể thêm các thuộc tính khác như tên người dùng, v.v.

        public ICommand ShowStudentManagementCommand { get; }

        public ICommand ShowAccountManagementCommand { get; }

        public ICommand ShowCheckupPlanCommand { get; }
        public ICommand ShowNurseDashboardCommand { get; }

        public ICommand ShowStudentLookupCommand { get; }
        public ICommand ShowDashboardCommand { get; }
        public ICommand LogoutCommand { get; }

        public ICommand ShowHomePageCommand { get; }



        public MainViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            ShowStudentManagementCommand = new RelayCommand(p => ShowStudentManagement());
            ShowAccountManagementCommand = new RelayCommand(p => ShowAccountManagement());
            ShowCheckupPlanCommand = new RelayCommand(p => ShowCheckupPlan());
            ShowNurseDashboardCommand = new RelayCommand(p => ShowNurseDashboard());
            ShowStudentLookupCommand = new RelayCommand(p => ShowStudentLookup());
            //ShowDashboardCommand = new RelayCommand(p => ShowDashboard());
            ShowDashboardCommand = new RelayCommand(p => ShowReport());
            ShowHomePageCommand = new RelayCommand(p => ShowHomePage());
            LogoutCommand = new RelayCommand(p => Logout());
        }

        private void Logout()
        {
            // Mở một cửa sổ đăng nhập mới
            var loginWindow = _serviceProvider.GetService<LoginWindow>();
            loginWindow.Show();

            // Tìm và đóng MainWindow hiện tại
            foreach (Window window in Application.Current.Windows)
            {
                if (window is MainWindow)
                {
                    window.Close();
                    break;
                }
            }
        }
        private void ShowStudentManagement()
        {
            CurrentView = _serviceProvider.GetService<StudentManagerViewModel>();
        }

        private void ShowAccountManagement()
        {
            CurrentView = _serviceProvider.GetService<AccountManagerViewModel>();
        }

        private void ShowCheckupPlan()
        {
            CurrentView = _serviceProvider.GetService<CheckupPlanViewModel>();
        }

        private void ShowNurseDashboard()
        {
            CurrentView = _serviceProvider.GetService<NurseDashboardViewModel>();
        }

        private void ShowStudentLookup()
        {
            CurrentView = _serviceProvider.GetService<StudentLookupViewModel>();
        }


        private void ShowReport() // Đổi tên cho rõ ràng
        {
            CurrentView = _serviceProvider.GetService<ReportViewModel>();
        }

        private async void ShowHomePage() // Thêm async và đổi tên
        {
            var homeViewModel = _serviceProvider.GetService<HomeViewModel>();
            await homeViewModel.InitializeAsync(_currentUser); // Thêm await
            CurrentView = homeViewModel;
        }

        public void Initialize(User user)
        {
            _currentUser = user;
            CurrentUserRole = user.Role;

            // Luôn hiển thị HomeView khi đăng nhập
            ShowHomePage();
            //ShowStudentManagement();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
