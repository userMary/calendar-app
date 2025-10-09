using CalendarApp.Mobile.Models;
using CalendarApp.Mobile.Services;
using Microsoft.Maui.Controls;

namespace CalendarApp.Mobile.Pages;

public partial class LoginPage : ContentPage
{
    private readonly ApiService _api;

    public LoginPage(ApiService apiService)
    {
        InitializeComponent();
        //_api = apiService;
        _api = apiService ?? throw new ArgumentNullException(nameof(apiService));
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        //messageLabel.Text = "";

        //var email = emailEntry.Text?.Trim() ?? "";
        //var pwd = passwordEntry.Text ?? "";

        //if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(pwd))
        //{
        //    messageLabel.Text = "Заполните все поля";
        //    return;
        //}

        //// вызываем API
        //var (user, error) = await _api.LoginAsync(email, pwd);
        //if (user == null)
        //{
        //    messageLabel.Text = string.IsNullOrWhiteSpace(error) ? "Ошибка входа" : error;
        //    return;
        //}

        //// сохраняем состояние
        //AppState.CurrentUser = user;

        //// Переходим на CalendarPage (её нужно создать)
        //await Navigation.PushAsync(new CalendarPage(_api));

        //var (user, error) = await _api.LoginAsync(emailEntry.Text, passwordEntry.Text);
        //if (user != null)
        //{
        //    AppState.CurrentUser = user;
        //    //await Navigation.PushAsync(new CalendarPage(_api));
        //    await Shell.Current.GoToAsync("//CalendarPage");
        //}
        //else
        //{
        //    await DisplayAlert("Ошибка", error ?? "Не удалось войти", "OK");
        //}

        var email = emailEntry.Text?.Trim();
        var password = passwordEntry.Text?.Trim();

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("Ошибка", "Введите email и пароль", "ОК");
            return;
        }

        var (user, error) = await _api.LoginAsync(email, password);

        if (user == null)
        {
            await DisplayAlert("Ошибка входа", error ?? "Неверные данные", "ОК");
            return;
        }

        AppState.CurrentUser = user;
        await Shell.Current.GoToAsync("//CalendarPage");

    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        //await Navigation.PushAsync(new RegisterPage(_api));
        //await Shell.Current.GoToAsync("RegisterPage");
        await Shell.Current.GoToAsync("//RegisterPage");
    }

    private void OnShowPasswordChanged(object sender, CheckedChangedEventArgs e)
    {
        passwordEntry.IsPassword = !e.Value;
    }

}
