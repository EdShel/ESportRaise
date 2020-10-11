using System.Collections.Generic;

namespace ESportRaise.BackEnd.DAL.Entities
{
    public sealed class Team
    {
        public int Id { set; get; }

        public string Name { set; get; }

        public int CoachId { set; get; }

        public Coach Coach { set; get; }

        public IList<TeamMember> Members { set; get; }
    }
}
