using ESportRaise.BackEnd.BLL.Constants;
using System.ComponentModel.DataAnnotations;

namespace ESportRaise.BackEnd.API.Models.Auth
{
    public sealed class RegisterRequest
    {
        [Required, RegularExpression(AuthConstants.EMAIL_REGEX)]
        public string Email { get; set; }

        [Required, RegularExpression(AuthConstants.USER_NAME_REGEX)]
        public string UserName { get; set; }

        [Required, RegularExpression(AuthConstants.PASSWORD_REGEX)]
        public string Password { get; set; }

        [Required, RegularExpression(@"^(Member)|(Admin)$")]
        public string Role { get; set; }
    }
}
