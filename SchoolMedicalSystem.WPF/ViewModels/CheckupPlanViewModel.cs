using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SchoolMedicalSystem.Core.Models;
using SchoolMedicalSystem.Core.Services;
using SchoolMedicalSystem.WPF.Views;
using System.Windows.Input;

namespace SchoolMedicalSystem.WPF.ViewModels
{
    public class CheckupPlanViewModel : INotifyPropertyChanged
    {
        private readonly ICheckupPlanService _planService;
        private readonly IServiceProvider _serviceProvider;

        public ObservableCollection<CheckupPlan> Plans { get; } = new ObservableCollection<CheckupPlan>();
        public CheckupPlan SelectedPlan { get; set; }

        public ICommand AddPlanCommand { get; }
        public ICommand EditPlanCommand { get; }

        public CheckupPlanViewModel(ICheckupPlanService planService, IServiceProvider serviceProvider)
        {
            _planService = planService;
            _serviceProvider = serviceProvider;
            AddPlanCommand = new RelayCommand(p => OpenPlanDetail(null));
            EditPlanCommand = new RelayCommand(p => OpenPlanDetail(SelectedPlan), p => SelectedPlan != null);
            _ = LoadPlansAsync();
        }

        private async Task LoadPlansAsync()
        {
            Plans.Clear();
            var plansFromDb = await _planService.GetAllPlansAsync();
            foreach (var plan in plansFromDb)
            {
                Plans.Add(plan);
            }
        }

        private void OpenPlanDetail(CheckupPlan plan)
        {
            var viewModel = _serviceProvider.GetService<PlanDetailViewModel>();
            viewModel.Initialize(plan);
            var window = _serviceProvider.GetService<PlanDetailWindow>();
            window.DataContext = viewModel;
            if (window.ShowDialog() == true)
            {
                _ = LoadPlansAsync();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
