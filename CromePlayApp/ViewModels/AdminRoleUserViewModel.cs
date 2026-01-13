using CromePlayApp.Models;
using Microsoft.AspNetCore.Identity;
using X.PagedList;

namespace CromePlayApp.ViewModels
{
    public class AdminRoleUserViewModel
    {
        public List<Club> Clubes { get; set; } = new List<Club>();

        public List<Game> Games { get; set; } = new List<Game>();

        public List<Member> Members { get; set; } = new List<Member>();

        public List<PendingUser> PendingUsers { get; set; } = new List<PendingUser>();

        public List<IdentityUser>? IdentityUsers { get; set; }


        public string FondoFileName { get; set; } = "fondo_4.jpg"; 

        public IPagedList<Game>? ClubsPaged { get; set; }






    }
}
