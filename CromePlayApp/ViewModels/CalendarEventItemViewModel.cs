namespace CromePlayApp.ViewModels
{
    public class CalendarEventItemViewModel
    {

        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public int AvailablePlace { get; set; }
        public int Price { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string? Type { get; set; }
        public bool IsAllDay { get; set; }

    }
}
