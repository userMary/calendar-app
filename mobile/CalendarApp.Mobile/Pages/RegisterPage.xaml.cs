using System.Text.RegularExpressions;
using CalendarApp.Mobile.Models;
using CalendarApp.Mobile.Services;
using Microsoft.Maui.Controls;

namespace CalendarApp.Mobile.Pages;

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

        // клиентска€ валидаци€ парол€
        var regex = new Regex(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@?!_\-+=*/]).{8,}$");
        if (!regex.IsMatch(pwd))
        {
            messageLabel.Text = "ѕароль: min 8, цифры, строчные и заглавные буквы, спецсимвол.";
            return;
        }

        var (user, error) = await _api.RegisterAsync(email, pwd, name);
        if (user == null)
        {
            messageLabel.Text = string.IsNullOrWhiteSpace(error) ? "ќшибка регистрации" : error;
            return;
        }

        // ”спех Ч логиним и переходим в календарь
        AppState.CurrentUser = user;
        await Navigation.PushAsync(new CalendarPage(_api));
    }
}
