using System.Text.RegularExpressions;
using CalendarApp.Mobile.Models;
using CalendarApp.Mobile.Services;
using Microsoft.Maui.Controls;

namespace CalendarApp.Mobile.Pages
{
    public partial class RegisterPage : ContentPage
    {
        private readonly ApiService _api;

        public RegisterPage(ApiService apiService)
        {
            InitializeComponent();
            _api = apiService;
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            messageLabel.Text = "";

            var email = emailEntry.Text?.Trim() ?? "";
            var name = nameEntry.Text?.Trim() ?? "";
            var pwd = passwordEntry.Text ?? "";

            // клиентская валидация пароля
            var regex = new Regex(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@?!_\-+=*/]).{8,}$");
            if (!regex.IsMatch(pwd))
            {
                messageLabel.Text = "Пароль: min 8, цифры, строчные и заглавные буквы, спецсимвол.";
                return;
            }

            var (user, error) = await _api.RegisterAsync(email, pwd, name);
            if (user == null)
            {
                messageLabel.Text = string.IsNullOrWhiteSpace(error) ? "Ошибка регистрации" : error;
                return;
            }

            // Успех — логиним и переходим в календарь
            AppState.CurrentUser = user;
            //await Navigation.PushAsync(new CalendarPage(_api));
            //await Shell.Current.GoToAsync("//CalendarPage");


            // регистрация и переход на CalendarPage
            await Shell.Current.DisplayAlert("Регистрация", "Регистрация прошла успешно!", "ОК");
            await Shell.Current.GoToAsync("//CalendarPage");
        }
    }
}
