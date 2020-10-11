namespace ESportRaise.BackEnd.DAL.Entities
{
    public sealed class TeamMember
    {
        public int Id { set; get; }

        public string Name { set; get; }

        public int TeamId { set; get; }

        public Team Team { set; get; }
    }
}
