using SchoolMedicalSystem.Core.Models;
using SchoolMedicalSystem.Core.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SchoolMedicalSystem.WPF.ViewModels
{
    public class StudentDetailViewModel : INotifyPropertyChanged
    {
        private readonly IStudentService _studentService;
        private bool _isEditMode;

        public Student Student { get; set; }
        public string WindowTitle { get; private set; }
        public ICommand SaveCommand { get; }
        public IEnumerable<Gender> Genders => Enum.GetValues(typeof(Gender)).Cast<Gender>();
        public List<int> Grades { get; } = new List<int> { 1, 2, 3, 4, 5 };
        public List<string> ClassSuffixes { get; } = new List<string> { "A", "B", "C" };
        public int SelectedGrade { get; set; }
        public string SelectedClassSuffix { get; set; }

        public StudentDetailViewModel(IStudentService studentService)
        {
            _studentService = studentService;
            SaveCommand = new RelayCommand(async (p) => await Save(p), (p) => CanSave());
        }

        public void Initialize(Student student = null)
        {
            if (student == null) // Chế độ Thêm mới
            {
                _isEditMode = false;
                WindowTitle = "Thêm Học Sinh Mới";
                Student = new Student { DateOfBirth = DateTime.Now, IsActive = true, StudentCode = "(Sẽ được tạo tự động)" };
                SelectedGrade = 1;
                SelectedClassSuffix = "A";
            }
            else // Chế độ Sửa
            {
                _isEditMode = true;
                WindowTitle = "Cập Nhật Thông Tin Học Sinh";

                Student = new Student
                {
                    Id = student.Id,
                    StudentCode = student.StudentCode,
                    FullName = student.FullName,
                    DateOfBirth = student.DateOfBirth,
                    Gender = student.Gender,
                    Address = student.Address,
                    ParentName = student.ParentName,
                    ParentPhoneNumber = student.ParentPhoneNumber,
                    Allergies = student.Allergies,
                    ClassName = student.ClassName,
                    IsActive = student.IsActive
                };
                // ========================

                if (!string.IsNullOrEmpty(student.ClassName))
                {
                    var match = Regex.Match(student.ClassName, @"(\d+)([A-Z]+)");
                    if (match.Success)
                    {
                        SelectedGrade = int.Parse(match.Groups[1].Value);
                        SelectedClassSuffix = match.Groups[2].Value;
                    }
                }
            }
        }

        private bool CanSave() => Student != null && !string.IsNullOrEmpty(Student.FullName);

        private async Task Save(object parameter)
        {
            Student.ClassName = $"{SelectedGrade}{SelectedClassSuffix}";

            if (_isEditMode)
            {
                await _studentService.UpdateStudentAsync(Student);
            }
            else
            {
                await _studentService.AddStudentAsync(Student);
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