// Services/ApiService.cs
using System.Net.Http;
using System.Net.Http.Json;
using CalendarApp.Desktop.Models;
using CalendarAppWinForms.Models;
using Newtonsoft.Json;

namespace CalendarApp.Desktop.Services
{
    public class ApiService
    {
        private readonly HttpClient _http;

        public ApiService(string baseUrl)
        {
            _http = new HttpClient { BaseAddress = new Uri(baseUrl) };
        }

        public async Task<UserDto?> LoginAsync(string email, string password)
        {
            //var payload = new LoginRequestDto { Email = email, PasswordHash = password };
            var payload = new LoginRequestDto { Email = email, Password = password };
            var res = await _http.PostAsJsonAsync("api/users/login", payload);
            if (!res.IsSuccessStatusCode) return null;
            return await res.Content.ReadFromJsonAsync<UserDto>();
        }

        public async Task<(bool Success, string? ErrorMessage)> RegisterAsync(RegisterRequest request)
        {
            var res = await _http.PostAsJsonAsync("api/users/registeruser", request);
            //var res = await _http.PostAsJsonAsync("api/Users/register", request);
            if (res.IsSuccessStatusCode)
                return (true, null);

            var error = await res.Content.ReadAsStringAsync();
            return (false, error);
        }
        //        public async Task<(bool Success, string? ErrorMessage)> RegisterAsync(RegisterRequest request)
        //{
        //    var res = await _http.PostAsJsonAsync("Users/registeruser", request);
        //    if (res.IsSuccessStatusCode)
        //        return (true, null);

        //    var error = await res.Content.ReadAsStringAsync();
        //    return (false, error);
        //}


        public async Task<List<NoteDto>> GetNotesForUserAsync(int userId)
        {
            var res = await _http.GetAsync($"api/notes/user/{userId}");
            res.EnsureSuccessStatusCode();
            return await res.Content.ReadFromJsonAsync<List<NoteDto>>() ?? new List<NoteDto>();
        }

        public async Task<NoteDto?> CreateNoteAsync(NoteDto note)
        {
            var res = await _http.PostAsJsonAsync("api/notes", note);
            if (!res.IsSuccessStatusCode) return null;
            return await res.Content.ReadFromJsonAsync<NoteDto>();
        }

        public async Task<bool> DeleteNoteAsync(int id)
        {
            try
            {
                var response = await _http.DeleteAsync($"api/notes/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при удалении заметки: {ex.Message}");
                return false;
            }
            //var res = await _http.DeleteAsync($"api/Notes/{id}");
            //return res.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateNoteAsync(int id, NoteDto note)
        {
            var res = await _http.PutAsJsonAsync($"api/notes/{id}", note);
            return res.IsSuccessStatusCode;
        }
        // UploadImageAsync при необходимости



        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var response = await _http.GetAsync($"api/users/{id}");
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UserDto>(json);
        }
    }
}
