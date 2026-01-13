using CromePlayApp.Models;

namespace CromePlayApp.Domain.Calendar
{
    public class CalendarEvent
    {

        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public int AvailablePlace {  get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int Price { get; set; }
        public string? Type { get; set; }
        public bool IsAllDay { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public int GameId { get; set; }
        public Game? Game { get; set; }

    }
}
