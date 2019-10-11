using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace DomainLoggingService.Model
{
    [DataContract]
    public class ComputerInfo
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Model { get; set; }

        [DataMember]
        public string OS { get; set; }

        [DataMember]
        public string SerialNumber { get; set; }

        [DataMember]
        public string CPU { get; set; }

        [DataMember]
        public uint CPUCores { get; set; }

        [DataMember]
        public uint RAMGB { get; set; }

        [DataMember]
        public DiskType DiskMediaType { get; set; }

        [DataMember]
        public uint DiskSize { get; set; }

        [DataMember]
        public string GFX { get; set; }

        [DataMember]
        public string TeamViewerId { get; set; }

        [DataMember]
        public DateTime? LastAlive { get; set; }
    }

    [DataContract]
    public enum DiskType
    {
        [EnumMember]
        Unknown = 1,
        [EnumMember]
        Unspecified = 2,
        [EnumMember]
        HDD = 3,
        [EnumMember]
        SSD = 4,
        [EnumMember]
        SCM = 5
    }
}