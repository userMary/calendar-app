namespace CalendarApp.Mobile.Models
{
    public class RegisterRequest
    {
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty; // НЕ хэш, обычный пароль — API сам захэширует
        public string Name { get; set; } = string.Empty;
    }
}
