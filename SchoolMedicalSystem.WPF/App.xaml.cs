using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using SchoolMedicalSystem.Core.Data;
using SchoolMedicalSystem.Core.Repositories;
using SchoolMedicalSystem.Core.Services;
using SchoolMedicalSystem.WPF.ViewModels;
using SchoolMedicalSystem.WPF.Views;

namespace SchoolMedicalSystem.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ServiceProvider _serviceProvider;

        public App()
        {
            ServiceCollection services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            // 1. Đăng ký DbContext
            services.AddDbContext<SchoolDbContext>();

            // 2. Đăng ký Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<ICheckupPlanRepository, CheckupPlanRepository>();
            services.AddScoped<IHealthRecordRepository, HealthRecordRepository>();

            // 3. Đăng ký Services
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IStudentService, StudentService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<ICheckupPlanService, CheckupPlanService>();
            services.AddTransient<IHealthRecordService, HealthRecordService>();

            // 4. Đăng ký ViewModels
            // MainViewModel PHẢI LÀ SINGLETON
            services.AddSingleton<MainViewModel>();

            // Các ViewModel khác là Transient
            services.AddTransient<StudentDetailViewModel>();
            services.AddTransient<PlanDetailViewModel>();
            services.AddTransient<CheckupExecutionViewModel>();
            services.AddTransient<StudentLookupViewModel>();
            services.AddTransient<AccountManagerViewModel>();

            // Các ViewModel cần inject IServiceProvider hoặc MainViewModel sẽ dùng factory
            services.AddTransient(provider => new LoginViewModel(
                provider.GetRequiredService<IAuthService>(),
                provider
            ));
            services.AddTransient(provider => new StudentManagerViewModel(
                provider.GetRequiredService<IStudentService>(),
                provider
            ));
            services.AddTransient(provider => new CheckupPlanViewModel(
                provider.GetRequiredService<ICheckupPlanService>(),
                provider
            ));
            services.AddTransient(provider => new NurseDashboardViewModel(
                provider.GetRequiredService<ICheckupPlanService>(),
                provider,
                provider.GetRequiredService<MainViewModel>() // Sẽ lấy đúng instance Singleton
            ));

            // 5. Đăng ký Views (Windows)
            services.AddTransient<LoginWindow>();
            services.AddTransient<MainWindow>();
            services.AddTransient<StudentDetailWindow>();
            services.AddTransient<PlanDetailWindow>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Lấy LoginWindow từ DI container và hiển thị nó
            var loginWindow = _serviceProvider.GetService<LoginWindow>();
            loginWindow.Show();
        }
    }
}
