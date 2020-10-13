using System.Collections.Generic;

namespace ESportRaise.BackEnd.DAL.Entities
{
    public sealed class TeamFullInfo
    {
        public Team Team { set; get; }

        public Coach Coach { set; get; }

        public IEnumerable<TeamMember> Members { set; get; }
    }
}
