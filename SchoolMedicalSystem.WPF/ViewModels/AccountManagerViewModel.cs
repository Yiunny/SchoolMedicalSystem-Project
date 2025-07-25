using SchoolMedicalSystem.Core.Models;
using SchoolMedicalSystem.Core.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SchoolMedicalSystem.WPF.ViewModels
{
    public class AccountManagerViewModel : INotifyPropertyChanged
    {
        private readonly IUserService _userService;

        private User _managerAccount;
        public User ManagerAccount { get => _managerAccount; set { _managerAccount = value; OnPropertyChanged(); } }

        private User _nurseAccount;
        public User NurseAccount { get => _nurseAccount; set { _nurseAccount = value; OnPropertyChanged(); } }

        public string NewManagerPassword { get; set; }
        public string NewNursePassword { get; set; }

        private string _statusMessage;
        public string StatusMessage { get => _statusMessage; set { _statusMessage = value; OnPropertyChanged(); } }

        public ICommand ChangeManagerPasswordCommand { get; }
        public ICommand ResetNursePasswordCommand { get; }

        public AccountManagerViewModel(IUserService userService)
        {
            _userService = userService;
            ChangeManagerPasswordCommand = new RelayCommand(async p => await ChangePassword(ManagerAccount, NewManagerPassword));
            ResetNursePasswordCommand = new RelayCommand(async p => await ChangePassword(NurseAccount, NewNursePassword));
            _ = LoadAccountsAsync();
        }

        private async Task LoadAccountsAsync()
        {
            ManagerAccount = await _userService.GetManagerAccountAsync();
            NurseAccount = await _userService.GetNurseAccountAsync();
        }

        private async Task ChangePassword(User account, string newPassword)
        {
            if (account == null || string.IsNullOrWhiteSpace(newPassword))
            {
                StatusMessage = "Vui lòng nhập mật khẩu mới.";
                return;
            }

            account.PasswordHash = newPassword; // Tạm thời gán trực tiếp
            await _userService.UpdateUserAsync(account);
            StatusMessage = $"Đã cập nhật mật khẩu cho tài khoản '{account.Username}' thành công!";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}