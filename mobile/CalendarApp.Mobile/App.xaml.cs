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
        //protected override async void OnStart()
        //{
        //    if (AppState.CurrentUser != null)
        //    {
        //        try
        //        {
        //            var user = await ApiService.GetUserByIdAsync(AppState.CurrentUser.Id);
        //        }
        //        catch (Exception ex)
        //        {
        //            AppState.CurrentUser = null;
        //            await Shell.Current.DisplayAlert("Профиль удалён", "Ваш профиль был удалён администратором.", "ОК");
        //            await Shell.Current.GoToAsync("//LoginPage");
        //        }
        //    }
        //}

    }

}
