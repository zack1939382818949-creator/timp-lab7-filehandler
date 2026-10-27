using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows;
using WpfApp1.Models;
using WpfApp1.Services;

namespace WpfApp1.ViewModel
{
    public partial class PasswordViewModel : ObservableObject

    {
        private readonly FileDataService _dataService = new();

        public event Action<Teacher>? PasswordAccepted;



        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoginingCommand))]
        [NotifyCanExecuteChangedFor(nameof(ClearInputCommand))]
        private string _login = "";

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoginingCommand))]
        [NotifyCanExecuteChangedFor(nameof(ClearInputCommand))]
        private string _password = "";

        [ObservableProperty]
        private string _statusMessage = "";



        [RelayCommand(CanExecute = nameof(CanLogin))]
        private void Logining()
        {
            if (!_dataService.TeachersFileExists) { StatusMessage = "Файл данных не найден."; return; }

            List<Teacher> teachers = _dataService.LoadTeachers();

           
            Teacher? found = teachers.FirstOrDefault(t =>
                string.Equals(t.Login, Login.Trim(), StringComparison.OrdinalIgnoreCase)
                && string.Equals(t.Password, Password, StringComparison.Ordinal));

            if (found != null)
            {
                StatusMessage = "Вход выполнен";
                PasswordAccepted?.Invoke(found);
            }
            else
            {
                StatusMessage = "Неверный логин или пароль";
                MessageBox.Show("Неверный логин или пароль!", "Предупреждение",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private bool CanLogin() => !string.IsNullOrWhiteSpace(Login) && !string.IsNullOrWhiteSpace(Password);

        [RelayCommand(CanExecute = nameof(CanLogin))]
        private void ClearInput() { Login = ""; Password = ""; StatusMessage = ""; }


    }
}
