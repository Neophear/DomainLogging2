using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLoggingService.Models.Exceptions
{
    public class DataAccessException : Exception
    {
        public DataAccessException(Exception innerException = null)
            : base("There was an error accessing the data", innerException)
        { }

        public DataAccessException(string message, Exception innerException = null)
            : base(message, innerException)
        { }
    }
}
