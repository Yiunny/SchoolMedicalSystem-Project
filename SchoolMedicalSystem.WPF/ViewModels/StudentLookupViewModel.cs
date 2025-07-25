using SchoolMedicalSystem.Core.Models;
using SchoolMedicalSystem.Core.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SchoolMedicalSystem.WPF.ViewModels
{
    public class StudentLookupViewModel : INotifyPropertyChanged
    {
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

        public StudentLookupViewModel(IStudentService studentService)
        {
            _studentService = studentService;
            SearchCommand = new RelayCommand(async p => await SearchStudents());
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
    }
}