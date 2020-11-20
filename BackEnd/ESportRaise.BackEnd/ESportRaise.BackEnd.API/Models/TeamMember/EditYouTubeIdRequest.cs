using System.ComponentModel.DataAnnotations;

namespace ESportRaise.BackEnd.API.Models.TeamMember
{
    public class EditYouTubeIdRequest
    {
        [Required]
        public int TeamMemberId { get; set; }

        public string ChannelUrl { get; set; }
    }
}
