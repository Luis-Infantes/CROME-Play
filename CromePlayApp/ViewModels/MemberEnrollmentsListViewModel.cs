using CromePlayApp.Models;

namespace CromePlayApp.ViewModels
{

    public class MemberEnrollmentsListViewModel
    {
        public int MemberId { get; set; }
        public int CalendarEventId { get; set; }
        public int GameId { get; set; }

        public string? EventTitle { get; set; }
        public DateTime? EventStart { get; set; }
        public DateTime? EventEnd { get; set; }
        public int? AvailablePlace { get; set; }

        public int Price { get; set; }
        public bool? IsAllDay { get; set; }
        public string? EventType { get; set; }
    }

}
