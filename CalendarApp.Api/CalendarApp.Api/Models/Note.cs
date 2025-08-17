using System.Text.Json.Serialization;

namespace CalendarApp.Models
{
    public class Note
    {
        public int Id { get; set; }                // ID �������
        public DateTime Date { get; set; }         // ����
        public string Title { get; set; } = string.Empty;          // ������� ��������
        public string Description { get; set; } = string.Empty;    // ������ ��������
        public string Color { get; set; } = "white";          // ���� �������
        public string ImageUrl { get; set; } = string.Empty;       // ������ �� ���� (�� �������)

        // ����� � �������������
        public int UserId { get; set; }

        [JsonIgnore] // ���������� Notes ��� ������������ User
        public User? User { get; set; }
    }
}
