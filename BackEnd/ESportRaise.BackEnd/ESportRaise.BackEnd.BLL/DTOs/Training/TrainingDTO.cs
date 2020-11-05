using System;

namespace ESportRaise.BackEnd.BLL.DTOs.Training
{
    public class TrainingDTO
    {
        public int Id { get; set; }

        public int TeamId { get; set; }

        public DateTime BeginTime { get; set; }
    }
}
