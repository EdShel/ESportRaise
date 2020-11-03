namespace ESportRaise.BackEnd.BLL.DTOs.Auth
{
    public sealed class RegisterServiceRequest
    {
        public string Email { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string Role { get; set; }
    }
}
