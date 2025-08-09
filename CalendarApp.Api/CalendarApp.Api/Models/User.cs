namespace CalendarApp.Models
{
    public class User
    {
        public int Id { get; set; }             // ID ������������
        public string Email { get; set; } = string.Empty;       // Email
        public string PasswordHash { get; set; } = string.Empty;// ������ (���� � ���� ����)
        public string Name { get; set; } = string.Empty;        // ��� (�� �������)
        public ICollection<Note> Notes { get; set; } = new List<Note>(); // ��������� �������
    }
}
