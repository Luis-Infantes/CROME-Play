using Microsoft.AspNetCore.Identity;

namespace CromePlayApp.Models
{
    public class Member
    {
        public int MemberId { get; set; }


        public string? IdentityUserId { get; set; }
        public IdentityUser? IdentityUser { get; set; }


        public string? MemberName { get; set; }
        public string? MemberAddress { get; set; }
        public string? MemberEmail { get; set; }
        public string? MemberPhone { get; set; }


        public ICollection<Enrollment> Enrollments { get; set; }= new List<Enrollment>();
    }
}
