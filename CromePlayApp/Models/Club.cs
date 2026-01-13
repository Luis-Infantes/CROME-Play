namespace CromePlayApp.Models
{
    public class Club
    {
        public int ClubId { get; set; }

        public string? ClubName { get; set; }

        public string ClubEmail { get; set; } = string.Empty;

        public string? ClubDescription { get; set; }

        public ICollection<Game>? Games { get; set; }

        public ICollection<TaskItem> TaskItems { get; set; } = new List<TaskItem>();
    }
}
