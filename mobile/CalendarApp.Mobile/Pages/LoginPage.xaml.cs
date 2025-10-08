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
        _api = apiService;
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        //messageLabel.Text = "";

        //var email = emailEntry.Text?.Trim() ?? "";
        //var pwd = passwordEntry.Text ?? "";

        //if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(pwd))
        //{
        //    messageLabel.Text = "��������� ��� ����";
        //    return;
        //}

        //// �������� API
        //var (user, error) = await _api.LoginAsync(email, pwd);
        //if (user == null)
        //{
        //    messageLabel.Text = string.IsNullOrWhiteSpace(error) ? "������ �����" : error;
        //    return;
        //}

        //// ��������� ���������
        //AppState.CurrentUser = user;

        //// ��������� �� CalendarPage (� ����� �������)
        //await Navigation.PushAsync(new CalendarPage(_api));

        var (user, error) = await _api.LoginAsync(emailEntry.Text, passwordEntry.Text);
        if (user != null)
        {
            AppState.CurrentUser = user;
            await Navigation.PushAsync(new CalendarPage(_api));
        }
        else
        {
            await DisplayAlert("������", error ?? "�� ������� �����", "OK");
        }

    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegisterPage(_api));
    }

    private void OnShowPasswordChanged(object sender, CheckedChangedEventArgs e)
    {
        passwordEntry.IsPassword = !e.Value;
    }

}
