using Newtonsoft.Json;
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
            this.errors = errors?.ToList() ?? new List<OperationError>();
        }

        [JsonIgnore]
        public bool Success { get => errors == null || errors.Count == 0; }

        [JsonIgnore]
        public IEnumerable<OperationError> Errors
        {
            get => errors;
            set => errors = value.ToList();
        }

        public ErrorProneOperationResponse AddError(string message)
        {
            var error = new OperationError(message);
            errors.Add(error);
            return this;
        }
    }

    public sealed class RegisterRequest
    {
        public string Email { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string Role { get; set; }
    }

    public sealed class RegisterResponse : ErrorProneOperationResponse
    {

    }

    public sealed class LoginRequest
    {
        public string EmailOrUserName { get; set; }

        public string Password { get; set; }
    }

    public sealed class LoginResponse : ErrorProneOperationResponse
    {
        public LoginResponse(IEnumerable<OperationError> errors = null) : base(errors)
        {
        }

        public string Token { get; set; }

        public string RefreshToken { get; set; }

        public string Email { get; set; }

        public string UserName { get; set; }
    }

    public sealed class TokenRefreshRequest
    {
        public string UserName { get; set; }

        public string RefreshToken { get; set; }
    }

    public sealed class TokenRefreshResponse : ErrorProneOperationResponse
    {
        public string Token { get; set; }

        public string RefreshToken { get; set; }

        public TokenRefreshResponse(string error)
            : base(new[] { new OperationError(error) })
        {
        }

        public TokenRefreshResponse(IEnumerable<OperationError> errors = null) : base(errors)
        {
        }
    }

    public sealed class TokenRevokeRequest
    {
        public string UserName { get; set; }

        public string RefreshToken { get; set; }
    }

    public sealed class TokenRevokeResponse : ErrorProneOperationResponse
    {
        public TokenRevokeResponse(string error)
             : base(new[] { new OperationError(error) })
        {

        }

        public TokenRevokeResponse(IEnumerable<OperationError> errors = null) : base(errors)
        {
        }
    }
}
