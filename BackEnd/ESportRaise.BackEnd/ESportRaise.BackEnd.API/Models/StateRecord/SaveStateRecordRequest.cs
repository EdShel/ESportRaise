namespace ESportRaise.BackEnd.API.Models.StateRecord
{
    public class SaveStateRecordRequest
    {
        public int TrainingId { get; set; }

        public int HeartRate { get; set; }

        public float Temperature { get; set; }
    }
}
