namespace CalendarApp.DTOs
{
    public class NoteRequest
    {
        public DateTime Date { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Color { get; set; }
        public string ImageUrl { get; set; }
        public int UserId { get; set; }
    }
}
