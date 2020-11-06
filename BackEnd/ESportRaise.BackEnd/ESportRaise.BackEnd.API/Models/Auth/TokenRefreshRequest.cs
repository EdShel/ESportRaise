using System.ComponentModel.DataAnnotations;

namespace ESportRaise.BackEnd.API.Models.Auth
{
    public class TokenRefreshRequest
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}
