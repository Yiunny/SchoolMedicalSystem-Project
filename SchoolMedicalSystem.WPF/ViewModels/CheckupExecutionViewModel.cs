using SchoolMedicalSystem.Core.Models;
using SchoolMedicalSystem.Core.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;

namespace SchoolMedicalSystem.WPF.ViewModels
{
    public class CheckupExecutionViewModel : INotifyPropertyChanged
    {
        private readonly IStudentService _studentService;
        private readonly IHealthRecordService _healthRecordService;
        private readonly ICheckupPlanService _planService;
        private readonly IServiceProvider _serviceProvider;
        private readonly MainViewModel _mainViewModel;

        private CheckupPlan _currentPlan;
        private StudentCheckupItemViewModel _selectedStudentItem;
        private HealthRecord _currentHealthRecord;
        private bool _isEditingRecord;
        public List<string> VisionLevels { get; } = new List<string>
            { "1/10", "2/10", "3/10", "4/10", "5/10", "6/10", "7/10", "8/10", "9/10", "10/10" };
        public ObservableCollection<StudentCheckupItemViewModel> Students { get; } = new ObservableCollection<StudentCheckupItemViewModel>();
        public string PlanTitle { get; private set; }
        public bool IsFormEnabled => SelectedStudentItem != null;

        public StudentCheckupItemViewModel SelectedStudentItem
        {
            get => _selectedStudentItem;
            set
            {
                _selectedStudentItem = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsFormEnabled));
                _ = LoadHealthRecordForSelectedStudent();
            }
        }

        public HealthRecord CurrentHealthRecord
        {
            get => _currentHealthRecord;
            set { _currentHealthRecord = value; OnPropertyChanged(); }
        }

        public ICommand SaveRecordCommand { get; }
        public ICommand CompletePlanCommand { get; }

        public CheckupExecutionViewModel(IStudentService studentService, IHealthRecordService healthRecordService, ICheckupPlanService planService, IServiceProvider serviceProvider, MainViewModel mainViewModel)
        {
            _studentService = studentService;
            _healthRecordService = healthRecordService;
            _planService = planService;
            _serviceProvider = serviceProvider;
            _mainViewModel = mainViewModel;

            SaveRecordCommand = new RelayCommand(async p => await SaveRecord(), p => CanSave());
            CompletePlanCommand = new RelayCommand(async p => await CompletePlan());
        }

        public async Task InitializeAsync(CheckupPlan plan)
        {
            _currentPlan = plan;
            PlanTitle = $"Danh sách học sinh lớp: {plan.ClassName}";
            OnPropertyChanged(nameof(PlanTitle));

            if (_currentPlan.Status == CheckupPlanStatus.Planned)
            {
                await _planService.UpdatePlanStatusAsync(_currentPlan.Id, CheckupPlanStatus.InProgress);
                _currentPlan.Status = CheckupPlanStatus.InProgress;
            }

            Students.Clear();
            var studentsInClass = await _studentService.GetStudentsByClassNameAsync(plan.ClassName);
            foreach (var student in studentsInClass)
            {
                var itemViewModel = new StudentCheckupItemViewModel(student);
                var existingRecord = await _healthRecordService.GetByPlanAndStudentIdAsync(_currentPlan.Id, student.Id);
                if (existingRecord != null)
                {
                    itemViewModel.IsCompleted = true;
                }
                Students.Add(itemViewModel);
            }
        }

        private async Task LoadHealthRecordForSelectedStudent()
        {
            if (SelectedStudentItem == null) return;

            var student = SelectedStudentItem.Student;
            var existingRecord = await _healthRecordService.GetByPlanAndStudentIdAsync(_currentPlan.Id, student.Id);

            if (existingRecord != null)
            {
                CurrentHealthRecord = existingRecord;
                _isEditingRecord = true;
            }
            else
            {
                CurrentHealthRecord = new HealthRecord { StudentId = student.Id };
                _isEditingRecord = false;
            }
        }

        private bool CanSave()
        {
            return SelectedStudentItem != null && CurrentHealthRecord != null && CurrentHealthRecord.Height > 0 && CurrentHealthRecord.Weight > 0;
        }

        private async Task SaveRecord()
        {
            CurrentHealthRecord.CheckupPlanId = _currentPlan.Id;
            CurrentHealthRecord.CheckupDate = DateTime.Now;
            CurrentHealthRecord.NurseId = 2; // Giả sử NurseId là 2

            if (_isEditingRecord)
            {
                await _healthRecordService.UpdateHealthRecordAsync(CurrentHealthRecord);
            }
            else
            {
                await _healthRecordService.CreateHealthRecordAsync(CurrentHealthRecord);
            }

            MessageBox.Show($"Đã lưu hồ sơ cho học sinh: {SelectedStudentItem.FullName}");

            SelectedStudentItem.IsCompleted = true;

            var nextStudent = Students.FirstOrDefault(s => !s.IsCompleted);
            if (nextStudent != null)
            {
                SelectedStudentItem = nextStudent;
            }
            else
            {
                MessageBox.Show("Đã khám cho tất cả học sinh trong danh sách!");
                SelectedStudentItem = null;
            }
        }

        private async Task CompletePlan()
        {
            if (MessageBox.Show("Bạn có chắc chắn muốn hoàn thành và đóng kế hoạch khám này?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                await _planService.UpdatePlanStatusAsync(_currentPlan.Id, CheckupPlanStatus.Completed);
                _mainViewModel.CurrentView = _serviceProvider.GetService<NurseDashboardViewModel>();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}