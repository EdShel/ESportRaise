using System;

namespace ESportRaise.BackEnd.DAL.Entities
{
    public class CriticalMoment
    {
        public long Id { get; set; }

        public int TrainingId { get; set; }

        public DateTime BeginTime { get; set; }

        public DateTime EndTime { get; set; }
    }
}
