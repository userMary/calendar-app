// Backend/DTOs/RegisterRequest.cs
namespace CalendarApp.DTOs
{
    public class RegisterRequest
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string Name { get; set; } = "";
    }

}
