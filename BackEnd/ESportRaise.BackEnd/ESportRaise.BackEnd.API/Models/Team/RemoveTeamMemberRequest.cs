using System.ComponentModel.DataAnnotations;

namespace ESportRaise.BackEnd.API.Models.Team
{
    public sealed class RemoveTeamMemberRequest
    {
        [Required]
        public int TeamId { get; set; }

        [Required]
        public string User { get; set; }
    }
}
