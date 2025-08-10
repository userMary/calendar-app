using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CalendarApp.Desktop.Services;
using CalendarAppWinForms.Models;
using Microsoft.VisualBasic.ApplicationServices;

namespace CalendarApp.Desktop.Forms
{
    public partial class RegisterForm : Form
    {
        //private readonly HttpClient _httpClient;
        private readonly ApiService _apiService;
        public RegisterForm()
        {
            InitializeComponent();
            //_httpClient = new HttpClient
            //{
            //    BaseAddress = new Uri("https://localhost:7105/api/")
            //};
            _apiService = new ApiService("https://localhost:7105/api");
        }

        private async void btnRegister_Click(object sender, EventArgs e)
        {
            // Проверка на пустые поля
            if (string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text) ||
                string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Заполните все поля", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var request = new RegisterRequest
            {
                Email = txtEmail.Text,
                Password = txtPassword.Text,
                Name = txtName.Text
            };

            //try
            //{
            //    var response = await _httpClient.PostAsJsonAsync("users/register", request);

            //    if (response.IsSuccessStatusCode)
            //    {
            //        MessageBox.Show("Регистрация прошла успешно! Теперь войдите в систему.",
            //            "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //        Close(); // Закрываем форму регистрации
            //    }
            //    else
            //    {
            //        var error = await response.Content.ReadAsStringAsync();
            //        MessageBox.Show($"Ошибка регистрации: {error}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }
            //}
            //catch
            //{
            //    MessageBox.Show("Не удалось подключиться к серверу", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}

            var (success, error) = await _apiService.RegisterAsync(request);

            if (success)
            {
                MessageBox.Show("Регистрация прошла успешно! Теперь войдите в систему.",
                    "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
            else
            {
                MessageBox.Show($"Ошибка регистрации: {error}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            var loginForm = new LoginForm();
            loginForm.Show();
            this.Hide();
        }
    }
}
