// Services/ApiService.cs
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using CalendarApp.Mobile.Models;

namespace CalendarApp.Mobile.Services
{
    public class ApiService
    {
        private readonly HttpClient _http;

        public ApiService(HttpClient httpClient)
        {
            _http = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        // Login
        public async Task<(UserDto? user, string? error)> LoginAsync(string email, string password)
        {
            //try
            //{
            //    var payload = new LoginRequestDto
            //    {
            //        Email = email,
            //        PasswordHash = password // подстрой под API (или Password, если надо)
            //    };

            //    var res = await _http.PostAsJsonAsync("Users/login", payload);

            //    if (!res.IsSuccessStatusCode)
            //    {
            //        var txt = await res.Content.ReadAsStringAsync();
            //        return (null, txt);
            //    }

            //    var user = await res.Content.ReadFromJsonAsync<UserDto>();
            //    return (user, null);
            //}
            //catch (Exception ex)
            //{
            //    return (null, ex.Message);
            //}
            try
            {
                var payload = new LoginRequestDto
                {
                    Email = email,
                    //было
                    //PasswordHash = password // или Password, если API требует
                    Password = password
                };

                // Ручная сериализация JSON
                var json = JsonSerializer.Serialize(payload);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");

                var res = await _http.PostAsync("Users/login", content);

                if (!res.IsSuccessStatusCode)
                {
                    var txt = await res.Content.ReadAsStringAsync();
                    return (null, txt);
                }

                var userJson = await res.Content.ReadAsStringAsync();
                var user = JsonSerializer.Deserialize<UserDto>(userJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return (user, null);
            }
            catch (Exception ex)
            {
                return (null, ex.Message);
            }
        }

        // Register
        public async Task<(UserDto? user, string? error)> RegisterAsync(string email, string password, string name)
        {
            try
            {
                var payload = new RegisterRequest
                {
                    Email = email,
                    PasswordHash = password,
                    Name = name
                };

                var res = await _http.PostAsJsonAsync("Users/register", payload);

                if (!res.IsSuccessStatusCode)
                {
                    var txt = await res.Content.ReadAsStringAsync();
                    return (null, txt);
                }

                var user = await res.Content.ReadFromJsonAsync<UserDto>();
                return (user, null);
            }
            catch (Exception ex)
            {
                return (null, ex.Message);
            }
        }

        // Получить заметки пользователя
        public async Task<List<Note>> GetNotesForUserAsync(int userId)
        {
            try
            {
                var res = await _http.GetAsync($"Notes/user/{userId}");
                res.EnsureSuccessStatusCode();
                var lst = await res.Content.ReadFromJsonAsync<List<Note>>();
                return lst ?? new List<Note>();
            }
            catch
            {
                return new List<Note>();
            }
        }

        // Создать заметку
        public async Task<(Note? note, string? error)> CreateNoteAsync(Note note)
        {
            try
            {
                var res = await _http.PostAsJsonAsync("Notes", note);
                if (!res.IsSuccessStatusCode)
                {
                    var txt = await res.Content.ReadAsStringAsync();
                    return (null, txt);
                }
                var created = await res.Content.ReadFromJsonAsync<Note>();
                return (created, null);
            }
            catch (Exception ex)
            {
                return (null, ex.Message);
            }
        }

        // Обновить заметку
        public async Task<(Note? note, string? error)> UpdateNoteAsync(Note note)
        {
            try
            {
                var res = await _http.PutAsJsonAsync($"Notes/{note.Id}", note);
                if (!res.IsSuccessStatusCode)
                {
                    var txt = await res.Content.ReadAsStringAsync();
                    return (null, txt);
                }
                var updated = await res.Content.ReadFromJsonAsync<Note>();
                return (updated, null);
            }
            catch (Exception ex)
            {
                return (null, ex.Message);
            }
        }

        // Удалить заметку
        public async Task<bool> DeleteNoteAsync(int id)
        {
            try
            {
                var res = await _http.DeleteAsync($"Notes/{id}");
                return res.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        // Загрузить изображение
        public async Task<(string? imageUrl, string? error)> UploadImageAsync(Stream imageStream, string fileName)
        {
            try
            {
                using var content = new MultipartFormDataContent();
                var fileContent = new StreamContent(imageStream);
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg"); // или другой MIME-тип
                content.Add(fileContent, "file", fileName);

                var res = await _http.PostAsync("Notes/upload", content);

                if (!res.IsSuccessStatusCode)
                {
                    var txt = await res.Content.ReadAsStringAsync();
                    return (null, txt);
                }

                // Предполагаем, что API возвращает JSON с URL
                var result = await res.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                if (result != null && result.ContainsKey("url"))
                {
                    return (result["url"], null);
                }

                return (null, "Неверный ответ сервера");
            }
            catch (Exception ex)
            {
                return (null, ex.Message);
            }
        }
    }
}
