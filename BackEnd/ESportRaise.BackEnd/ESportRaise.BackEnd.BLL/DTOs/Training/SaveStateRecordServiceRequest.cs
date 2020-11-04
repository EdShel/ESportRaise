using System;
using System.Collections.Generic;
using System.Text;

namespace ESportRaise.BackEnd.BLL.DTOs.Training
{
    public class SaveStateRecordServiceRequest
    {
        public int TeamMemberId { get; set; }

        public int TrainingId { get; set; }

        public int HeartRate { get; set; }

        public float Temperature { get; set; }
    }
}
