using System.Collections.Generic;

namespace ESportRaise.BackEnd.DAL.Entities
{
    public sealed class Team
    {
        public int Id { set; get; }

        public string Name { set; get; }

        public int CoachId { set; get; }

    }
}
