namespace CalendarApp.Models
{
    public class Admin
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty; // пока без хэша
        public string Name { get; set; } = string.Empty;
    }
}
