using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace DomainLoggingService.Model
{
    [DataContract]
    public class LogEntry
    {
        [DataMember]
        public long Id { get; set; }
        [DataMember]
        public DateTime Timestamp { get; set; }
        [DataMember]
        public string Username { get; set; }
        [DataMember]
        public string Parameters { get; set; }
        [DataMember]
        public int ComputerId { get; set; }
        [DataMember]
        public string SerialNumber { get; set; }
    }
}