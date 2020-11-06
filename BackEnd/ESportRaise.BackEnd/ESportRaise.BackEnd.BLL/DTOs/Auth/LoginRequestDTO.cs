namespace ESportRaise.BackEnd.BLL.DTOs.Auth
{
    public sealed class LoginRequestDTO
    {
        public string EmailOrUserName { get; set; }

        public string Password { get; set; }
    }
}
