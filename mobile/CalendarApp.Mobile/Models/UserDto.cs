// Models/UserDto.cs
namespace CalendarApp.Mobile.Models
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        // PasswordHash �� �������� � DTO ��� �����������,
        // ����� ����� ���������� ����� LoginRequestDto
    }
}
