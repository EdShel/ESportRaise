namespace ESportRaise.BackEnd.BLL.DTOs.Auth
{
    public sealed class TokenServiceRefreshRequest
    {
        public string UserName { get; set; }

        public string RefreshToken { get; set; }
    }
}
