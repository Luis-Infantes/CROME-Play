namespace CromePlayApp.Models
{
    public class Enrollment
    {
        public int EnrollmentId { get; set; }
        public int GameId { get; set; }

        public Game? Game { get; set; }

        public int MemberId { get; set; }

        public Member? Member { get; set; }
    }
}
