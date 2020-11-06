using System.ComponentModel.DataAnnotations;

namespace ESportRaise.BackEnd.API.Models.StateRecord
{
    public class SaveStateRecordRequest
    {
        [Required]
        public int TrainingId { get; set; }

        [Required]
        public int HeartRate { get; set; }

        [Required]
        public float Temperature { get; set; }
    }
}
