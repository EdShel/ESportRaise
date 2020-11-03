namespace ESportRaise.BackEnd.BLL.DTOs.Auth
{
    public sealed class TokenServiceRevokeRequest
    {
        public string UserName { get; set; }

        public string RefreshToken { get; set; }
    }
}
