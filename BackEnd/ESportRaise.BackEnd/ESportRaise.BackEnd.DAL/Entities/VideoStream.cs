namespace ESportRaise.BackEnd.DAL.Entities
{
    public class VideoStream
    {
        public int Id { get; set; }

        public int TrainingId { get; set; }

        public int TeamMemberId { get; set; }

        public string YouTubeId { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }
    }
}
