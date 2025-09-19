namespace CalendarApp.DTOs
{
    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        // было PasswordHash
        // должно быть обычный пароль
        public string Password { get; set; } = string.Empty;
    }
}