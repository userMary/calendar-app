namespace CalendarApp.DTOs
{
    public class LoginRequestAdmin
    {
        public string Email { get; set; } = string.Empty;
        // было PasswordHash
        // должно быть обычный пароль
        public string Password { get; set; } = string.Empty;
    }
}