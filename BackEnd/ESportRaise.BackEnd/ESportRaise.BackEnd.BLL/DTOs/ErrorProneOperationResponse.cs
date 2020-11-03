using System.Collections.Generic;
using System.Linq;

namespace ESportRaise.BackEnd.BLL.DTOs
{
    public abstract class ErrorProneOperationResponse
    {
        private IList<OperationError> errors;

        protected ErrorProneOperationResponse(IEnumerable<OperationError> errors = null)
        {
            this.errors = errors?.ToList() ?? new List<OperationError>();
        }

        protected ErrorProneOperationResponse(string error)
        {
            this.errors = new[] { new OperationError(error) }.ToList();
        }

        public bool Success { get => errors == null || errors.Count == 0; }

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

}
