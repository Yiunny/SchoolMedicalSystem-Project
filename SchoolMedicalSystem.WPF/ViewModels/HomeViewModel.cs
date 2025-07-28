using SchoolMedicalSystem.Core.Models;
using SchoolMedicalSystem.Core.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace SchoolMedicalSystem.WPF.ViewModels
{
    public class HomeViewModel : INotifyPropertyChanged
    {
        private readonly IStudentService _studentService;
        private readonly ICheckupPlanService _planService;

        public string WelcomeMessage { get; set; }
        private string _currentDateTime;
        public string CurrentDateTime { get => _currentDateTime; set { _currentDateTime = value; OnPropertyChanged(); } }

        // Thuộc tính cho Manager
        public int TotalStudents { get; set; }

        // Thuộc tính cho Nurse
        public ObservableCollection<CheckupPlan> TodaysPlans { get; } = new ObservableCollection<CheckupPlan>();

        public HomeViewModel(IStudentService studentService, ICheckupPlanService planService)
        {
            _studentService = studentService;
            _planService = planService;

            // Cập nhật đồng hồ mỗi giây
            DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += (s, e) => { CurrentDateTime = DateTime.Now.ToString("dddd, dd/MM/yyyy HH:mm:ss"); };
            timer.Start();
        }

        public async Task InitializeAsync(User user)
        {
            WelcomeMessage = $"Chào mừng, {user.Username}!";
            OnPropertyChanged(nameof(WelcomeMessage));

            // Tải dữ liệu tương ứng với vai trò
            if (user.Role == UserRole.Manager)
            {
                var students = await _studentService.GetAllStudentsAsync();
                TotalStudents = students.Count();
                OnPropertyChanged(nameof(TotalStudents));
            }
            else if (user.Role == UserRole.Nurse)
            {
                var plans = await _planService.GetAllPlansAsync();
                var todaysPlans = plans.Where(p => p.CheckupDate.Date == DateTime.Today.Date && p.Status != CheckupPlanStatus.Completed);
                TodaysPlans.Clear();
                foreach (var plan in todaysPlans)
                {
                    TodaysPlans.Add(plan);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}