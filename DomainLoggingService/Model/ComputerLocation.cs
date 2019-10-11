using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace DomainLoggingService.Model
{
    [DataContract]
    public class ComputerLocation
    {
        [DataMember]
        public string MYN { get; set; }
        [DataMember]
        public string BYG { get; set; }
        [DataMember]
        public string LOK { get; set; }
        [DataMember]
        public string Owner { get; set; }
        [DataMember]
        public DateTime? LastUpdate { get; set; }
    }
}