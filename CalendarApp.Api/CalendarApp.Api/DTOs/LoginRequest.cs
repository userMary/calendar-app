namespace CalendarApp.DTOs
{
    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        // ���� PasswordHash
        // ������ ���� ������� ������
        public string Password { get; set; } = string.Empty;
    }
}