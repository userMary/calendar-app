// Models/LoginRequestDto.cs
namespace CalendarApp.Desktop.Models
{
    public class LoginRequestDto
    {
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty; // �������� ��� API
    }
}
