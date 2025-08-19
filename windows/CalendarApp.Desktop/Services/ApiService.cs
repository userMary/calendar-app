// Services/ApiService.cs
using System.Net.Http;
using System.Net.Http.Json;
using CalendarApp.Desktop.Models;
using CalendarAppWinForms.Models;

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
            var payload = new LoginRequestDto { Email = email, PasswordHash = password };
            var res = await _http.PostAsJsonAsync("api/Users/login", payload);
            if (!res.IsSuccessStatusCode) return null;
            return await res.Content.ReadFromJsonAsync<UserDto>();
        }

        public async Task<(bool Success, string? ErrorMessage)> RegisterAsync(RegisterRequest request)
        {
            var res = await _http.PostAsJsonAsync("api/Users/registeruser", request);
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
            var res = await _http.GetAsync($"api/Notes/user/{userId}");
            res.EnsureSuccessStatusCode();
            return await res.Content.ReadFromJsonAsync<List<NoteDto>>() ?? new List<NoteDto>();
        }

        public async Task<NoteDto?> CreateNoteAsync(NoteDto note)
        {
            var res = await _http.PostAsJsonAsync("api/Notes", note);
            if (!res.IsSuccessStatusCode) return null;
            return await res.Content.ReadFromJsonAsync<NoteDto>();
        }

        public async Task<bool> DeleteNoteAsync(int id)
        {
            var res = await _http.DeleteAsync($"api/Notes/{id}");
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateNoteAsync(int id, NoteDto note)
        {
            var res = await _http.PutAsJsonAsync($"api/Notes/{id}", note);
            return res.IsSuccessStatusCode;
        }
        // UploadImageAsync при необходимости
    }
}
