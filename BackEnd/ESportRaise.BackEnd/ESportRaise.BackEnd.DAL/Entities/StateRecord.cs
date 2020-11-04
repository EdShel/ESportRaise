using System;

namespace ESportRaise.BackEnd.DAL.Entities
{
    public class StateRecord
    {
        public int Id { get; set; }

        public int TrainingId { get; set; }

        public int TeamMemberId { get; set; }

        public DateTime CreateTime { get; set; }

        public int HeartRate { get; set; }

        public float Temperature { get; set; }
    }
}
