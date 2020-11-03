using System.Collections.Generic;

namespace ESportRaise.BackEnd.BLL.DTOs.Auth
{
    public sealed class LoginServiceResponse : ErrorProneOperationResponse
    {
        public LoginServiceResponse(IEnumerable<OperationError> errors = null) : base(errors)
        {
        }

        public LoginServiceResponse(string error) : base(error)
        {
        }

        public string Token { get; set; }

        public string RefreshToken { get; set; }

        public string Email { get; set; }

        public string UserName { get; set; }
    }
}
