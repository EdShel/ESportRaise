namespace ESportRaise.BackEnd.API.Models.Team
{
    public sealed class AddTeamMemberRequest
    {
        public int TeamId { get; set; }

        public string User { get; set; }
    }
}
