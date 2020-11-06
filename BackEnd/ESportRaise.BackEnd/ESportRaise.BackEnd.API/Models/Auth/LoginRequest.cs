using ESportRaise.BackEnd.BLL.Constants;
using System.ComponentModel.DataAnnotations;

namespace ESportRaise.BackEnd.API.Models.Auth
{
    public sealed class LoginRequest
    {
        [Required]
        public string EmailOrUserName { get; set; }

        [Required, RegularExpression(AuthConstants.PASSWORD_REGEX)]
        public string Password { get; set; }
    }
}
