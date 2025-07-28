using Microsoft.Extensions.DependencyInjection;
using SchoolMedicalSystem.Core.Models;
using SchoolMedicalSystem.Core.Services;
using SchoolMedicalSystem.WPF.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SchoolMedicalSystem.WPF.ViewModels
{
    public class StudentLookupViewModel : INotifyPropertyChanged
    {

        private readonly IServiceProvider _serviceProvider;
        private readonly IStudentService _studentService;
        private string _searchTerm;
        private Student _selectedStudent;


        public string SearchTerm
        {
            get => _searchTerm;
            set { _searchTerm = value; OnPropertyChanged(); }
        }

        public Student SelectedStudent
        {
            get => _selectedStudent;
            set
            {
                _selectedStudent = value;
                OnPropertyChanged();
                // Thông báo cho thuộc tính IsStudentSelected cũng thay đổi
                OnPropertyChanged(nameof(IsStudentSelected));
            }
        }

        // Thuộc tính bool mới để điều khiển việc ẩn/hiện
        public bool IsStudentSelected => SelectedStudent != null;

        public ObservableCollection<Student> SearchResults { get; } = new ObservableCollection<Student>();
        public ICommand SearchCommand { get; }
        public ICommand ViewHistoryCommand { get; }

        public StudentLookupViewModel(IStudentService studentService, IServiceProvider serviceProvider)  
        {
            _studentService = studentService;
            _serviceProvider = serviceProvider;  
            SearchCommand = new RelayCommand(async p => await SearchStudents());
            ViewHistoryCommand = new RelayCommand(async p => await OpenHistoryWindow(), p => SelectedStudent != null);  
            _ = SearchStudents();
        }

        private async Task SearchStudents()
        {
            SearchResults.Clear();
            var students = await _studentService.SearchStudentsAsync(SearchTerm);
            foreach (var student in students)
            {
                SearchResults.Add(student);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private async Task OpenHistoryWindow()
        {
            var viewModel = _serviceProvider.GetService<StudentHistoryViewModel>();
            await viewModel.LoadHistoryAsync(SelectedStudent);

            var window = _serviceProvider.GetService<StudentHistoryWindow>();
            window.DataContext = viewModel;
            window.Owner = Application.Current.MainWindow;
            window.Show();
        }
    }
}