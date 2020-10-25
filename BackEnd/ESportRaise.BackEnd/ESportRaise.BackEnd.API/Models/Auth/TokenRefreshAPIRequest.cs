namespace ESportRaise.BackEnd.API.Models.Auth
{
    public class TokenRefreshAPIRequest
    {
        public string RefreshToken { get; set; }
    }

    public class TokenRevokeAPIRequest
    {
        public string RefreshToken { get; set; }
    }
}
