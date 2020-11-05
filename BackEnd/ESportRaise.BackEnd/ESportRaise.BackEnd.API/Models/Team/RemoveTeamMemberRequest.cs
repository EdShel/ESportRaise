namespace ESportRaise.BackEnd.API.Models.Team
{
    public sealed class RemoveTeamMemberRequest
    {
        public int TeamId { get; set; }

        public string User { get; set; }
    }
}
