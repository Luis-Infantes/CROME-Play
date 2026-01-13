namespace CromePlayApp.ViewModels
{

    public class EventMembersViewModel
    {
        public int EventId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        // Lista de miembros inscritos
        public List<MemberItem> Members { get; set; } = new();
    }

    public class MemberItem
    {
        public int MemberId { get; set; }
        public string MemberName { get; set; } = string.Empty;
        public string? MemberEmail { get; set; }
        public string? MemberAddress { get; set; }
        public string? MemberPhone { get; set; }
    }

}
