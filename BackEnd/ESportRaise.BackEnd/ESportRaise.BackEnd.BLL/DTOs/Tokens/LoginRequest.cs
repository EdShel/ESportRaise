using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ESportRaise.BackEnd.BLL.DTOs.Tokens
{
    public class OperationError
    {
        public OperationError(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }

    public abstract class ErrorProneOperationResponse
    {
        private IList<OperationError> errors;

        protected ErrorProneOperationResponse(IEnumerable<OperationError> errors = null)
        {
            Success = !errors?.Any() ?? true;
            errors = errors?.ToList() ?? new List<OperationError>();
        }

        public bool Success { get; }

        public IEnumerable<OperationError> Errors { get => errors; }

        public ErrorProneOperationResponse AddError(string message)
        {
            var error = new OperationError(message);
            errors.Add(error);
            return this;
        }
    }

    public sealed class LoginRequest
    {
        public string EmailOrUserName { get; set; }

        public string Password { get; set; }
    }

    public sealed class LoginResponse : ErrorProneOperationResponse
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
