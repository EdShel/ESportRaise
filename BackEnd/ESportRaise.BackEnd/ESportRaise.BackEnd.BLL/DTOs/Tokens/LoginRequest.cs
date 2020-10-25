using System;
using System.Collections.Generic;
using System.Text;

namespace ESportRaise.BackEnd.BLL.DTOs.Tokens
{
    public sealed class LoginRequest
    {
        public string EmailOrUserName { get; set; }

        public string Password { get; set; }
    }

    public sealed class LoginResponse
    {
        public string Token { get; set; }

        public string RefreshToken { get; set; }

        public string Email { get; set; }

        public string UserName { get; set; }
    }

    public sealed class RegisterRequest
    {

    }

    public sealed class RegisterResponse
    {

    }

    public sealed class TokenRefreshRequest
    {

    }

    public sealed class TokenRefreshResponse
    {

    }

    public sealed class TokenRevokeRequest
    {

    }

    public sealed class TokenRevokeResponse
    {

    }
}
