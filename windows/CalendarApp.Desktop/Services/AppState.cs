// AppState.cs
using CalendarApp.Desktop.Models;
using CalendarApp.Desktop.Services;

namespace CalendarApp.Desktop
{
    public static class AppState
    {
        // Укажи адрес своего API (например https://localhost:7105/)
        //public static ApiService Api { get; set; } = new ApiService("https://localhost:7105/api");
        public static ApiService Api { get; set; } = new ApiService("http://localhost:7105/api");
        public static UserDto? CurrentUser { get; set; }
    }
}
