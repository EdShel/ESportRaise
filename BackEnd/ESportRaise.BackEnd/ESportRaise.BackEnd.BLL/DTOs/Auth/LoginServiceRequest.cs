namespace ESportRaise.BackEnd.BLL.DTOs.Auth
{
    public sealed class LoginServiceRequest
    {
        public string EmailOrUserName { get; set; }

        public string Password { get; set; }
    }
}
