using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using MAVPC.Services;
using System.Windows.Controls;

namespace MAVPC.MVVM.ViewModels
{
    public class LoginSuccessMessage : ValueChangedMessage<string>
    {
        public LoginSuccessMessage(string user) : base(user) { }
    }

    public partial class LoginViewModel : ObservableObject
    {
        private readonly IAuthService _authService;

        [ObservableProperty] private string _username = string.Empty;
        [ObservableProperty] private string _errorMessage = string.Empty;
        [ObservableProperty] private bool _hasError;

        public LoginViewModel(IAuthService authService)
        {
            _authService = authService;
        }

        [RelayCommand]
        private void Login(object parameter)
        {
            var passwordBox = parameter as PasswordBox;
            var password = passwordBox?.Password;

            if (_authService.Login(Username, password))
            {
                HasError = false;
                WeakReferenceMessenger.Default.Send(new LoginSuccessMessage(Username));
            }
            else
            {
                ErrorMessage = "Usuario o contraseña incorrectos";
                HasError = true;
            }
        }
    }
}