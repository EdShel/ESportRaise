using System;

namespace ESportRaise.BackEnd.BLL.DTOs.CriticalMoment
{
    public class CriticalMomentDTO
    {
        public long Id { get; set; }

        public int TrainingId { get; set; }

        public DateTime BeginTime { get; set; }

        public DateTime EndTime { get; set; }

    }
}
