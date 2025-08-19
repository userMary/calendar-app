using System;
using System.Windows.Forms;
using CalendarApp.Desktop.Forms;
using CalendarApp.Desktop.Services;

namespace CalendarApp.Desktop
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            //ApiService api = ;
            //// Получите userId после логина (ниже — временно для теста)
            //int userId = 1;
            //Application.Run(new CalendarForm(api, userId));

            Application.Run(new LoginForm());
        }
    }
}