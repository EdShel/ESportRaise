using System;
using System.Collections.Generic;

namespace ESportRaise.BackEnd.BLL.DTOs.StateRecord
{
    public class GetStateRecordServiceResponse
    {
        public int TrainingId { get; set; }

        public IEnumerable<StateRecord> StateRecords { get; set; }

        public class StateRecord
        {
            public int TeamMemberId { get; set; }

            public DateTime CreateTime { get; set; }

            public int HeartRate { get; set; }

            public float Temperature { get; set; }
        }
    }
}
