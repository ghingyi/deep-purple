using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deeP.Abstraction
{
    public enum RepositoryErrorCode
    {
        General,
        Validation,
        Conflict,
        Unauthorized,
        NotFound
    }

    public class RepositoryException : ApplicationException
    {
        public RepositoryErrorCode ErrorCode { get; private set; }

        public RepositoryException(RepositoryErrorCode errorCode, string message, Exception innerException = null)
            : base(message, innerException)
        {
            this.ErrorCode = errorCode;
        }
    }
}
