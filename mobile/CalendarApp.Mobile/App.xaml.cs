using Microsoft.Maui.Controls;
using CalendarApp.Mobile.Services;

namespace CalendarApp.Mobile
{
    public partial class App : Application
    {
        public ApiService ApiService { get; }
        public App()
        {
            InitializeComponent();

            // Получаем ApiService через DI
            ApiService = MauiProgram.Services.GetRequiredService<ApiService>();

            // Навигация на LoginPage с передачей ApiService
            MainPage = new NavigationPage(new Pages.LoginPage(ApiService));
        }
    }
}
