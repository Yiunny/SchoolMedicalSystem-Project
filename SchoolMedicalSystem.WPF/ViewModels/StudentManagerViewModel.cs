using Microsoft.Extensions.DependencyInjection;
using SchoolMedicalSystem.Core.Models;
using SchoolMedicalSystem.Core.Services;
using SchoolMedicalSystem.WPF.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SchoolMedicalSystem.WPF.ViewModels
{
    public class StudentManagerViewModel : INotifyPropertyChanged
    {
        private readonly IStudentService _studentService;
        private readonly IServiceProvider _serviceProvider;

        // Dùng một danh sách gốc để chứa tất cả dữ liệu từ DB
        private List<Student> _allStudents;

        private Student _selectedStudent;
        private string _searchName;
        private string _selectedClass;

        public ObservableCollection<Student> Students { get; }
        public ObservableCollection<string> ClassList { get; }

        public Student SelectedStudent
        {
            get => _selectedStudent;
            set { _selectedStudent = value; OnPropertyChanged(); }
        }

        public string SearchName
        {
            get => _searchName;
            set { _searchName = value; OnPropertyChanged(); }
        }

        public string SelectedClass
        {
            get => _selectedClass;
            set { _selectedClass = value; OnPropertyChanged(); }
        }

        public ICommand AddStudentCommand { get; }
        public ICommand EditStudentCommand { get; }
        public ICommand DeleteStudentCommand { get; }
        public ICommand FilterCommand { get; }
        public ICommand ClearFilterCommand { get; }

        public StudentManagerViewModel(IStudentService studentService, IServiceProvider serviceProvider)
        {
            _studentService = studentService;
            _serviceProvider = serviceProvider;
            Students = new ObservableCollection<Student>();
            ClassList = new ObservableCollection<string>();
            _allStudents = new List<Student>();

            AddStudentCommand = new RelayCommand(p => OpenStudentDetail(null));
            EditStudentCommand = new RelayCommand(p => OpenStudentDetail(SelectedStudent), p => SelectedStudent != null);
            DeleteStudentCommand = new RelayCommand(async p => await DeleteStudent(), p => SelectedStudent != null);

            FilterCommand = new RelayCommand(p => ApplyFilter());
            ClearFilterCommand = new RelayCommand(p => ClearFilter());

            _ = LoadDataAsync();
        }

        private void OpenStudentDetail(Student student)
        {
            var viewModel = _serviceProvider.GetService<StudentDetailViewModel>();
            viewModel.Initialize(student);
            var window = _serviceProvider.GetService<StudentDetailWindow>();
            window.DataContext = viewModel;
            if (window.ShowDialog() == true)
            {
                _ = LoadDataAsync(); // Tải lại toàn bộ dữ liệu nếu có thay đổi
            }
        }

        private async Task DeleteStudent()
        {
            // ... logic xóa giữ nguyên ...
            if (MessageBox.Show($"Bạn có chắc chắn muốn xóa học sinh '{SelectedStudent.FullName}'?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                await _studentService.DeleteStudentAsync(SelectedStudent.Id);
                await LoadDataAsync(); // Tải lại toàn bộ dữ liệu
            }
        }

        private async Task LoadDataAsync()
        {
            _allStudents = (await _studentService.GetAllStudentsAsync()).ToList();

            // Cập nhật danh sách lớp học cho ComboBox
            var classNames = _allStudents.Select(s => s.ClassName).Distinct().OrderBy(c => c).ToList();
            ClassList.Clear();
            foreach (var className in classNames)
            {
                ClassList.Add(className);
            }

            ApplyFilter();
        }

        private void ApplyFilter()
        {
            // Bắt đầu với danh sách đầy đủ
            IEnumerable<Student> filteredStudents = _allStudents;

            // Lọc theo tên (không phân biệt hoa thường)
            if (!string.IsNullOrWhiteSpace(SearchName))
            {
                filteredStudents = filteredStudents.Where(s => s.FullName.Contains(SearchName, StringComparison.OrdinalIgnoreCase));
            }

            // Lọc theo lớp
            if (!string.IsNullOrWhiteSpace(SelectedClass))
            {
                filteredStudents = filteredStudents.Where(s => s.ClassName == SelectedClass);
            }

            // Cập nhật lại danh sách hiển thị trên Grid
            Students.Clear();
            foreach (var student in filteredStudents)
            {
                Students.Add(student);
            }
        }

        private void ClearFilter()
        {
            SearchName = string.Empty;
            SelectedClass = null; // hoặc string.Empty tùy vào ComboBox
            ApplyFilter();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}