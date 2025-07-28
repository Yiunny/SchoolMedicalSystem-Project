using LiveCharts;
using LiveCharts.Wpf;
using SchoolMedicalSystem.Core.Models;
using SchoolMedicalSystem.Core.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SchoolMedicalSystem.WPF.ViewModels
{
    public class StudentHistoryViewModel : INotifyPropertyChanged
    {
        private readonly IHealthRecordService _healthRecordService;

        private string _windowTitle;
        public string WindowTitle { get => _windowTitle; set { _windowTitle = value; OnPropertyChanged(); } }

        public ObservableCollection<HealthRecord> HealthRecords { get; } = new ObservableCollection<HealthRecord>();

        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }

        public StudentHistoryViewModel(IHealthRecordService healthRecordService)
        {
            _healthRecordService = healthRecordService;
            SeriesCollection = new SeriesCollection();
        }

        public async Task LoadHistoryAsync(Student student)
        {
            WindowTitle = $"Lịch sử Khám bệnh: {student.FullName} ({student.StudentCode})";
            var records = (await _healthRecordService.GetHistoryByStudentIdAsync(student.Id)).ToList();

            HealthRecords.Clear();
            foreach (var record in records) { HealthRecords.Add(record); }

            Labels = HealthRecords.Select(r => r.CheckupDate.ToString("dd/MM/yy")).ToArray();
            SeriesCollection.Clear();
            SeriesCollection.Add(new LineSeries
            {
                Title = "Cân nặng (kg)",
                Values = new ChartValues<double>(HealthRecords.Select(r => r.Weight))
            });
            SeriesCollection.Add(new LineSeries
            {
                Title = "Chiều cao (cm)",
                Values = new ChartValues<double>(HealthRecords.Select(r => r.Height))
            });
            SeriesCollection.Add(new LineSeries
            {
                Title = "BMI",
                Values = new ChartValues<double>(HealthRecords.Select(r => r.Bmi))
            });

            OnPropertyChanged(nameof(Labels));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}