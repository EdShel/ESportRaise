using System.ComponentModel.DataAnnotations;

namespace ESportRaise.BackEnd.API.Models.Auth
{
    public class TokenRevokeRequest
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}
