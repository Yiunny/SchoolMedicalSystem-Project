using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SchoolMedicalSystem.Core.Models;

namespace SchoolMedicalSystem.WPF.ViewModels
{
    public class StudentCheckupItemViewModel : INotifyPropertyChanged
    {
        public Student Student { get; }

        private bool _isCompleted;
        public bool IsCompleted
        {
            get => _isCompleted;
            set { _isCompleted = value; OnPropertyChanged(); }
        }

        // Các thuộc tính tiện ích để binding trên UI dễ dàng hơn
        public string FullName => Student.FullName;
        public int StudentId => Student.Id;

        public StudentCheckupItemViewModel(Student student)
        {
            Student = student;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
