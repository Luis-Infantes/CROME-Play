using Microsoft.AspNetCore.Identity;

namespace CromePlayApp.Models
{
    public class PendingUser
    {
        public int PendingUserId { get; set; }

        public string? IdentityUserId { get; set; }
        public IdentityUser? IdentityUser { get; set; }

        public string? PendingUserEmail { get; set; }

        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    }
}
