using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using CalendarApp.Desktop.Services;
using CalendarApp.Desktop.Models;
using System.Net.Http;
using System.Net.Http.Json;

namespace CalendarApp.Desktop.Forms
{
    public partial class LoginForm : Form
    {
        private readonly HttpClient _httpClient;
        public LoginForm()
        {
            InitializeComponent();
            _httpClient = new HttpClient
            {
                //BaseAddress = new Uri("https://localhost:7105/api/") // твой адрес API
                BaseAddress = new Uri("http://localhost:7105/api/") // твой адрес API
            };
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            btnLogin.Enabled = false;
            lblError.Text = "";

            try
            {
                var email = txtEmail.Text.Trim();
                var password = txtPassword.Text; // пока используем plain, бек преобразует

                var user = await AppState.Api.LoginAsync(email, password);
                if (user == null)
                {
                    lblError.Text = "Неверный логин или пароль";
                    return;
                }

                AppState.CurrentUser = user;

                // Открыть CalendarForm и скрыть LoginForm
                var calendar = new CalendarForm(AppState.Api);
                calendar.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                lblError.Text = "Ошибка: " + ex.Message;
            }
            finally
            {
                btnLogin.Enabled = true;
            }
        }




        // Регистрация
        private void btnRegister_Click(object sender, EventArgs e)
        {
            // Можешь открыть окно регистрации (или вызывать API регистрации)
            var registerForm = new RegisterForm();
            //registerForm.ShowDialog(); // Модальное окно
            registerForm.Show();
            this.Hide();
        }
    }
}
