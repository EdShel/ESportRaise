using System;

namespace ESportRaise.BackEnd.DAL.Entities
{
    public sealed class Training
    {
        public int Id { get; set; }
        
        public int TeamId { get; set; }

        public DateTime BeginTime { get; set; }
    }
}
