using System.ComponentModel.DataAnnotations;

namespace ESportRaise.BackEnd.API.Models.Team
{
    public sealed class TeamCreateRequest
    {
        [Required, RegularExpression("^[A-Za-z0-9_@]{4,30}$")]
        public string TeamName { get; set; }
    }
}
