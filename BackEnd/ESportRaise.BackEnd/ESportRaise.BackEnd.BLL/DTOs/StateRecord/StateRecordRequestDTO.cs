namespace ESportRaise.BackEnd.BLL.DTOs.StateRecord
{
    public class StateRecordRequestDTO
    {
        public int TrainingId { get; set; }

        public int TimeInSeconds { get; set; }

        public int? TeamMemberId { get; set; }
    }
}
