using System.Collections.Generic;

namespace ESportRaise.BackEnd.BLL.DTOs.Auth
{
    public sealed class TokenServiceRefreshResponse
    {
        public string Token { get; set; }

        public string RefreshToken { get; set; }
    }
}
