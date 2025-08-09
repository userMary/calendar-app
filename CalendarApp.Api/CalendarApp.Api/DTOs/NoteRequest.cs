namespace CalendarApp.DTOs
{
    public class NoteRequest
    {
        public DateTime Date { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Color { get; set; } = "white";
        public string ImageUrl { get; set; } = string.Empty;
        public int UserId { get; set; }
    }
}
