// Models/NoteDto.cs
namespace CalendarApp.Desktop.Models
{
    public class NoteDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }           // DateTime на клиенте
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Color { get; set; } = "white";
        public string ImageUrl { get; set; } = string.Empty;
        public int UserId { get; set; }
    }
}
