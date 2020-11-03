using System.Collections.Generic;

namespace ESportRaise.BackEnd.BLL.DTOs.Auth
{
    public sealed class LoginServiceResponse
    {
        public string Token { get; set; }

        public string RefreshToken { get; set; }

        public string Email { get; set; }

        public string UserName { get; set; }
    }
}
