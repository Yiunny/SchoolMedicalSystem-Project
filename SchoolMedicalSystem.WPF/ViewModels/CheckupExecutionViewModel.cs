using SchoolMedicalSystem.Core.Models;
using SchoolMedicalSystem.Core.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection; // Thêm using này

namespace SchoolMedicalSystem.WPF.ViewModels
{
    public class CheckupExecutionViewModel : INotifyPropertyChanged
    {
        private readonly IStudentService _studentService;
        private readonly IHealthRecordService _healthRecordService;
        private readonly ICheckupPlanService _planService; // Thêm service mới
        private readonly IServiceProvider _serviceProvider; // Thêm service provider
        private readonly MainViewModel _mainViewModel; // Thêm MainViewModel

        private CheckupPlan _currentPlan;
        private Student _selectedStudent;
        private HealthRecord _currentHealthRecord;
        private bool _isEditingRecord; // Cờ để biết đang sửa hay thêm mới

        public ObservableCollection<Student> Students { get; } = new ObservableCollection<Student>();
        public string PlanTitle { get; private set; }
        public bool IsFormEnabled => SelectedStudent != null;

        public Student SelectedStudent
        {
            get => _selectedStudent;
            set
            {
                _selectedStudent = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsFormEnabled));
                // Khi chọn một học sinh, kiểm tra xem đã có hồ sơ chưa
                _ = LoadHealthRecordForSelectedStudent();
            }
        }

        public HealthRecord CurrentHealthRecord
        {
            get => _currentHealthRecord;
            set { _currentHealthRecord = value; OnPropertyChanged(); }
        }

        public ICommand SaveRecordCommand { get; }
        public ICommand CompletePlanCommand { get; } // Command mới

        public CheckupExecutionViewModel(IStudentService studentService, IHealthRecordService healthRecordService, ICheckupPlanService planService, IServiceProvider serviceProvider, MainViewModel mainViewModel)
        {
            _studentService = studentService;
            _healthRecordService = healthRecordService;
            _planService = planService; // Gán service mới
            _serviceProvider = serviceProvider;
            _mainViewModel = mainViewModel;

            SaveRecordCommand = new RelayCommand(async p => await SaveRecord(), p => CanSave());
            CompletePlanCommand = new RelayCommand(async p => await CompletePlan()); // Khởi tạo command mới
        }

        public async Task InitializeAsync(CheckupPlan plan)
        {
            _currentPlan = plan;
            PlanTitle = $"Danh sách học sinh lớp: {plan.ClassName}";
            OnPropertyChanged(nameof(PlanTitle));

            // Cập nhật trạng thái kế hoạch thành "Đang tiến hành"
            if (_currentPlan.Status == CheckupPlanStatus.Planned)
            {
                await _planService.UpdatePlanStatusAsync(_currentPlan.Id, CheckupPlanStatus.InProgress);
                _currentPlan.Status = CheckupPlanStatus.InProgress;
            }

            Students.Clear();
            var studentsInClass = await _studentService.GetStudentsByClassNameAsync(plan.ClassName);
            foreach (var student in studentsInClass)
            {
                Students.Add(student);
            }
        }

        private async Task LoadHealthRecordForSelectedStudent()
        {
            if (_selectedStudent == null) return;

            // Tìm hồ sơ đã có cho học sinh này trong kế hoạch này
            var existingRecord = await _healthRecordService.GetByPlanAndStudentIdAsync(_currentPlan.Id, _selectedStudent.Id);

            if (existingRecord != null)
            {
                // Nếu có, tải lên để sửa
                CurrentHealthRecord = existingRecord;
                _isEditingRecord = true;
            }
            else
            {
                // Nếu không, tạo một hồ sơ mới để nhập liệu
                CurrentHealthRecord = new HealthRecord { StudentId = _selectedStudent.Id };
                _isEditingRecord = false;
            }
        }

        private bool CanSave()
        {
            return SelectedStudent != null && CurrentHealthRecord != null && CurrentHealthRecord.Height > 0 && CurrentHealthRecord.Weight > 0;
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

            MessageBox.Show($"Đã lưu hồ sơ cho học sinh: {SelectedStudent.FullName}");

            // Tự động chuyển sang học sinh tiếp theo
            int currentIndex = Students.IndexOf(SelectedStudent);
            if (currentIndex < Students.Count - 1)
            {
                SelectedStudent = Students[currentIndex + 1];
            }
            else
            {
                MessageBox.Show("Đã khám cho học sinh cuối cùng trong danh sách!");
                SelectedStudent = null;
            }
        }

        private async Task CompletePlan()
        {
            if (MessageBox.Show("Bạn có chắc chắn muốn hoàn thành và đóng kế hoạch khám này?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                await _planService.UpdatePlanStatusAsync(_currentPlan.Id, CheckupPlanStatus.Completed);
                // Quay trở về màn hình Dashboard của Y tá
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