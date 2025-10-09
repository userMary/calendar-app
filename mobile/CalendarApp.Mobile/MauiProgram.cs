// Запуск C:\Users\maryg>adb reverse tcp:7105 tcp:7105
// ответ 7105

//C: \Users\maryg > adb devices
//List of devices attached
//0I73C10I23100F79        device

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using CalendarApp.Mobile.Services;
using System.Net.Http;
using System;
using CalendarApp.Mobile.Pages;
//using CalendarApp.Mobile.Views;

namespace CalendarApp.Mobile;

public static class MauiProgram
{
    public static IServiceProvider Services { get; private set; } = default!;

    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // Создаём HttpClient с BaseAddress и передаём в ApiService
        //builder.Services.AddSingleton(new ApiService(new HttpClient
        //{
        //    BaseAddress = new Uri("https://10.0.2.2:7105/api") // твой локальный API
        //}));


        //builder.Services.AddHttpClient("API", client =>
        //{
        //    client.BaseAddress = new Uri("http://10.0.2.2:7105/api/");
        //});
        builder.Services.AddHttpClient<ApiService>(client =>
        {
            //client.BaseAddress = new Uri("http://10.0.2.2:7105/api/");
            //client.BaseAddress = new Uri("http://194.164.33.248:7105/api/");
            //client.BaseAddress = new Uri("http://192.168.1.17:7105/api/");
            client.BaseAddress = new Uri("http://localhost:7105/api/");
        });







        // Регистрация страниц
        builder.Services.AddTransient<CalendarPage>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<NoteModalPage>();
        builder.Services.AddTransient<RegisterPage>();

        var app = builder.Build();

        // Сохраняем сервис-провайдер
        Services = app.Services;

        return app;
    }
}
