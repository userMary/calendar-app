// Services/ApiService.cs
using System.Net.Http;
using System.Net.Http.Json;
using CalendarApp.Desktop.Models;

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

        // ... добавь UpdateNoteAsync, UploadImageAsync при необходимости
    }
}
