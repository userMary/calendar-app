using System.Text.Json.Serialization;

namespace CalendarApp.Models
{
    public class Note
    {
        public int Id { get; set; }                // ID заметки
        public DateTime Date { get; set; }         // Дата
        public string Title { get; set; } = string.Empty;          // Краткое описание
        public string Description { get; set; } = string.Empty;    // Полное описание
        public string Color { get; set; } = "white";          // Цвет заметки
        public string ImageUrl { get; set; } = string.Empty;       // Ссылка на фото (по желанию)

        // Связь с пользователем
        public int UserId { get; set; }

        [JsonIgnore] // Игнорируем Notes при сериализации User
        public User? User { get; set; }
    }
}
