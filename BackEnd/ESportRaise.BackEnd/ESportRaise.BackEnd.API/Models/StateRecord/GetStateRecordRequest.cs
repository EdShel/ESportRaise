using System.ComponentModel.DataAnnotations;

namespace ESportRaise.BackEnd.API.Models.StateRecord
{
    public class GetStateRecordRequest
    {
        [Required]
        public int TrainingId { get; set; }

        public int? UserId { get; set; }

        [Required]
        public int TimeInSecs { get; set; }
    }
}
