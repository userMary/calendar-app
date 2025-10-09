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

            // ���������� ��������� ������
            var regex = new Regex(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@?!_\-+=*/]).{8,}$");
            if (!regex.IsMatch(pwd))
            {
                messageLabel.Text = "������: min 8, �����, �������� � ��������� �����, ����������.";
                return;
            }

            var (user, error) = await _api.RegisterAsync(email, pwd, name);
            if (user == null)
            {
                messageLabel.Text = string.IsNullOrWhiteSpace(error) ? "������ �����������" : error;
                return;
            }

            // ����� � ������� � ��������� � ���������
            AppState.CurrentUser = user;
            //await Navigation.PushAsync(new CalendarPage(_api));
            //await Shell.Current.GoToAsync("//CalendarPage");


            // ����������� � ������� �� CalendarPage
            await Shell.Current.DisplayAlert("�����������", "����������� ������ �������!", "��");
            await Shell.Current.GoToAsync("//CalendarPage");
        }
    }
}
