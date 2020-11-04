namespace ESportRaise.BackEnd.API.Models.Training
{
    public class SaveStateRecordRequest
    {
        public int UserId { get; set; }

        public int TrainingId { get; set; }

        public int HeartRate { get; set; }

        public float Temperature { get; set; }
    }
}
