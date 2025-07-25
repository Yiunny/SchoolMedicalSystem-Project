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
using Microsoft.Extensions.DependencyInjection;

namespace SchoolMedicalSystem.WPF.ViewModels
{
    public class NurseDashboardViewModel : INotifyPropertyChanged
    {
        private readonly ICheckupPlanService _planService;
        private readonly IServiceProvider _serviceProvider;
        private readonly MainViewModel _mainViewModel;

        // === PHẦN SỬA LỖI BẮT ĐẦU ===
        private CheckupPlan _selectedPlan;

        public ObservableCollection<CheckupPlan> Plans { get; } = new ObservableCollection<CheckupPlan>();

        public CheckupPlan SelectedPlan
        {
            get => _selectedPlan;
            set
            {
                _selectedPlan = value;
                OnPropertyChanged(); // <-- THÔNG BÁO CHO UI VÀ COMMAND RẰNG GIÁ TRỊ ĐÃ THAY ĐỔI
            }
        }
        // === PHẦN SỬA LỖI KẾT THÚC ===

        public ICommand StartCheckupCommand { get; }

        public NurseDashboardViewModel(ICheckupPlanService planService, IServiceProvider serviceProvider, MainViewModel mainViewModel)
        {
            _planService = planService;
            _serviceProvider = serviceProvider;
            _mainViewModel = mainViewModel;
            StartCheckupCommand = new RelayCommand(async p => await StartCheckup(), p => SelectedPlan != null);
            _ = LoadPlansAsync();
        }

        private async Task LoadPlansAsync()
        {
            Plans.Clear();
            var plansFromDb = await _planService.GetAllPlansAsync();

            var activePlans = plansFromDb.Where(p => p.Status != CheckupPlanStatus.Completed);

            foreach (var plan in activePlans)
            {
                Plans.Add(plan);
            }
        }

        private async Task StartCheckup()
        {
            var executionViewModel = _serviceProvider.GetService<CheckupExecutionViewModel>();
            await executionViewModel.InitializeAsync(SelectedPlan);

            _mainViewModel.CurrentView = executionViewModel;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}