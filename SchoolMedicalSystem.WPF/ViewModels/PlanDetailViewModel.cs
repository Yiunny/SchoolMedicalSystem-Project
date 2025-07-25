using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SchoolMedicalSystem.Core.Models;
using SchoolMedicalSystem.Core.Services;
using System.Windows.Input;
using System.Windows;

namespace SchoolMedicalSystem.WPF.ViewModels
{
    public class PlanDetailViewModel : INotifyPropertyChanged
    {
        private readonly ICheckupPlanService _planService;
        private readonly IStudentService _studentService;
        private bool _isEditMode;

        public CheckupPlan Plan { get; set; }
        public string WindowTitle { get; private set; }
        public ObservableCollection<string> ClassList { get; } = new ObservableCollection<string>();
        public ICommand SaveCommand { get; }

        public IEnumerable<CheckupPlanStatus> Statuses => Enum.GetValues(typeof(CheckupPlanStatus)).Cast<CheckupPlanStatus>();
        public PlanDetailViewModel(ICheckupPlanService planService, IStudentService studentService)
        {
            _planService = planService;
            _studentService = studentService;
            SaveCommand = new RelayCommand(async (p) => await Save(p));
            _ = LoadClassListAsync();
        }

        private async Task LoadClassListAsync()
        {
            var students = await _studentService.GetAllStudentsAsync();
            var classNames = students.Select(s => s.ClassName).Distinct().OrderBy(c => c);
            ClassList.Clear();
            foreach (var name in classNames)
            {
                ClassList.Add(name);
            }
        }

        public void Initialize(CheckupPlan plan = null)
        {
            if (plan == null)
            {
                _isEditMode = false;
                WindowTitle = "Tạo Kế hoạch Khám mới";
                Plan = new CheckupPlan { CheckupDate = DateTime.Now };
            }
            else
            {
                _isEditMode = true;
                WindowTitle = "Cập nhật Kế hoạch Khám";
                Plan = new CheckupPlan
                {
                    Id = plan.Id,
                    PlanName = plan.PlanName,
                    ClassName = plan.ClassName,
                    CheckupDate = plan.CheckupDate,
                    Status = plan.Status
                };
            }
        }

        private async Task Save(object parameter)
        {
            if (_isEditMode)
            {
                await _planService.UpdatePlanAsync(Plan);
            }
            else
            {
                await _planService.AddPlanAsync(Plan);
            }
            if (parameter is Window window)
            {
                window.DialogResult = true;
                window.Close();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
