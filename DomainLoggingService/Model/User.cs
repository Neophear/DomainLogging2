using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace DomainLoggingService.Model
{
    [DataContract]
    public class User
    {
        [DataMember]
        public string Username { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public bool Locked { get; set; }
    }
}