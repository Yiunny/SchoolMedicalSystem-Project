using LiveCharts;
using LiveCharts.Wpf;
using SchoolMedicalSystem.Core.Models;
using SchoolMedicalSystem.Core.Services;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace SchoolMedicalSystem.WPF.ViewModels
{
    public class ReportViewModel : INotifyPropertyChanged
    {
        private readonly IStudentService _studentService;
        private readonly ICheckupPlanService _planService;
        private readonly IHealthRecordService _healthRecordService;

        private int _totalStudents;
        public int TotalStudents { get => _totalStudents; set { _totalStudents = value; OnPropertyChanged(); } }

        private int _totalHealthRecords;
        public int TotalHealthRecords { get => _totalHealthRecords; set { _totalHealthRecords = value; OnPropertyChanged(); } }

        private int _totalPlans;
        public int TotalPlans { get => _totalPlans; set { _totalPlans = value; OnPropertyChanged(); } }

        private int _completedPlans;
        public int CompletedPlans { get => _completedPlans; set { _completedPlans = value; OnPropertyChanged(); } }

        public SeriesCollection BmiDistributionSeries { get; set; }
        public SeriesCollection CheckupsByClassSeries { get; set; }

        private string[] _classLabels;
        public string[] ClassLabels { get => _classLabels; set { _classLabels = value; OnPropertyChanged(); } }

        public Func<double, string> Formatter { get; set; } = value => value.ToString("N0");

        public ReportViewModel(IStudentService studentService, ICheckupPlanService planService, IHealthRecordService healthRecordService)
        {
            _studentService = studentService;
            _planService = planService;
            _healthRecordService = healthRecordService;
            BmiDistributionSeries = new SeriesCollection();
            CheckupsByClassSeries = new SeriesCollection();

            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                // Lấy dữ liệu thô từ các service
                var students = (await _studentService.GetAllStudentsAsync()).ToList();
                var plans = (await _planService.GetAllPlansAsync()).ToList();
                var allHealthRecords = new List<HealthRecord>();
                foreach (var student in students)
                {
                    var records = await _healthRecordService.GetHistoryByStudentIdAsync(student.Id);
                    allHealthRecords.AddRange(records);
                }

                // Gói toàn bộ phần cập nhật giao diện vào Dispatcher để đảm bảo an toàn
                Application.Current.Dispatcher.Invoke(() =>
                {
                    // 1. Cập nhật các thẻ thống kê
                    TotalStudents = students.Count;
                    TotalHealthRecords = allHealthRecords.Count;
                    TotalPlans = plans.Count;
                    CompletedPlans = plans.Count(p => p.Status == CheckupPlanStatus.Completed);

                    // 2. Chuẩn bị dữ liệu cho biểu đồ tròn BMI
                    var bmiGroups = allHealthRecords
                        .Where(r => !string.IsNullOrEmpty(r.BmiStatus) && r.BmiStatus != "Không xác định")
                        .GroupBy(r => r.BmiStatus)
                        .Select(g => new PieSeries { Title = g.Key, Values = new ChartValues<int> { g.Count() }, DataLabels = true });

                    BmiDistributionSeries.Clear();
                    foreach (var group in bmiGroups) { BmiDistributionSeries.Add(group); }

                    // 3. Chuẩn bị dữ liệu cho biểu đồ cột
                    var checkupsByClass = allHealthRecords
                        .Join(students, hr => hr.StudentId, s => s.Id, (hr, s) => s.ClassName)
                        .GroupBy(className => className)
                        .Select(g => new { ClassName = g.Key, Count = g.Count() })
                        .OrderBy(x => x.ClassName).ToList();

                    ClassLabels = checkupsByClass.Select(x => x.ClassName).ToArray();
                    CheckupsByClassSeries.Clear();
                    CheckupsByClassSeries.Add(new ColumnSeries
                    {
                        Title = "Số lượt khám",
                        Values = new ChartValues<int>(checkupsByClass.Select(x => x.Count))
                    });
                });
            }
            catch (Exception ex)
            {
                // Hiển thị lỗi nếu có vấn đề xảy ra
                MessageBox.Show($"Đã xảy ra lỗi khi tải dữ liệu Báo cáo: {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}