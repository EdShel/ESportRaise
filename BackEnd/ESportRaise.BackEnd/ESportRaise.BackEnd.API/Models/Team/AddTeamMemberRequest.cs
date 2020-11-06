using System.ComponentModel.DataAnnotations;

namespace ESportRaise.BackEnd.API.Models.Team
{
    public sealed class AddTeamMemberRequest
    {
        [Required]
        public int TeamId { get; set; }

        [Required, MinLength(2)]
        public string User { get; set; }
    }
}
