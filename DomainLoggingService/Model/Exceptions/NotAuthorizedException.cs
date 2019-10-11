using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLoggingService.Models.Exceptions
{
    public class NotAuthorizedException : Exception
    {
        public NotAuthorizedException(string message = "Not authorized.")
            : base(message) { }
    }
}
