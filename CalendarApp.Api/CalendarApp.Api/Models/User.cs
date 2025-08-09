namespace CalendarApp.Models
{
    public class User
    {
        public int Id { get; set; }             // ID пользователя
        public string Email { get; set; } = string.Empty;       // Email
        public string PasswordHash { get; set; } = string.Empty;// Пароль (пока в виде хэша)
        public string Name { get; set; } = string.Empty;        // Имя (по желанию)
        public ICollection<Note> Notes { get; set; } = new List<Note>(); // Связанные заметки
    }
}
