using System.Collections.Generic;

namespace ESportRaise.BackEnd.BLL.DTOs.Auth
{
    public sealed class TokenServiceRefreshResponse : ErrorProneOperationResponse
    {
        public string Token { get; set; }

        public string RefreshToken { get; set; }

        public TokenServiceRefreshResponse(string error)
            : base(new[] { new OperationError(error) })
        {
        }

        public TokenServiceRefreshResponse(IEnumerable<OperationError> errors = null) : base(errors)
        {
        }
    }
}
