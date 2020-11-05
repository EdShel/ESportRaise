using System.ComponentModel.DataAnnotations;

namespace ESportRaise.BackEnd.API.Models.TeamMember
{
    public class EditYouTubeIdRequest
    {
        [Required]
        public string ChannelUrl { get; set; }
    }
}
