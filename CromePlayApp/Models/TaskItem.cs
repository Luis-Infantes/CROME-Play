namespace CromePlayApp.Models
{
    public class TaskItem
    {
        public int Id { get; set; }

        public string Description { get; set; } = string.Empty;

        public bool IsCompleted { get; set; } = false;

        public int ClubId { get; set; }

        public Club? Club { get; set; }
    }
}
