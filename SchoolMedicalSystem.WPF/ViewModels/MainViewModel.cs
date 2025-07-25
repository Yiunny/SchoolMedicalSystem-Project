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

namespace SchoolMedicalSystem.WPF.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {

        private readonly IServiceProvider _serviceProvider;
        private object _currentView;

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
        public MainViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            ShowStudentManagementCommand = new RelayCommand(p => ShowStudentManagement());
            ShowAccountManagementCommand = new RelayCommand(p => ShowAccountManagement());
            ShowCheckupPlanCommand = new RelayCommand(p => ShowCheckupPlan());
            ShowNurseDashboardCommand = new RelayCommand(p => ShowNurseDashboard());
            ShowStudentLookupCommand = new RelayCommand(p => ShowStudentLookup());
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

        public void Initialize(UserRole userRole)
        {
            CurrentUserRole = userRole;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
