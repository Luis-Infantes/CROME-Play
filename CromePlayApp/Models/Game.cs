using CromePlayApp.Domain.Calendar;

namespace CromePlayApp.Models
{
    public class Game
    {

        public int GameId { get; set; }

        public string? GameName { get; set; }

        public string? GameDescription { get; set; }

        public int ClubId { get; set; }

        public Club? Club { get; set; }

        public ICollection<Enrollment>? Enrollments { get; set; }

        public ICollection<CalendarEvent>? CalendarEvents { get; set; } = new List<CalendarEvent>();
    }
}
