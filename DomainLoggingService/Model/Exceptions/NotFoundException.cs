using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLoggingService.Models.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message = "Object was not found!")
            : base(message) { }
    }
}
