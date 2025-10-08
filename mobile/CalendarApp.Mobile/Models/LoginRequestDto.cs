// Models/LoginRequestDto.cs
namespace CalendarApp.Mobile.Models
{
    public class LoginRequestDto
    {
        public string Email { get; set; } = string.Empty;
        //было
        //public string PasswordHash { get; set; } = string.Empty; // подстрой под API
        public string Password { get; set; } = string.Empty;
    }
}
