using System.Collections.Generic;

namespace ESportRaise.BackEnd.BLL.DTOs.Team
{
    public class TeamDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<TeamMember> Members { get; set; }

        public class TeamMember
        {
            public int Id { get; set; }

            public string UserName { get; set; }
        }
    }

}
