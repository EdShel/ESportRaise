namespace ESportRaise.BackEnd.BLL.DTOs.Training
{
    public class VideoStreamDTO
    {
        public int Id { get; set; }

        public int TeamMemberId { get; set; }

        public string StreamId { get; set; }

        public string StartTime { get; set; }
    }
}
