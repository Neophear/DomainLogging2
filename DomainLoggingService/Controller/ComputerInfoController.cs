using DomainLoggingService.Data;
using DomainLoggingService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DomainLoggingService.Controller
{
    public class ComputerInfoController
    {
        readonly IComputerInfoData data;

        public ComputerInfoController(IComputerInfoData data = null)
        {
            this.data = data ?? new ComputerInfoData();
        }

        public void SaveComputerInfo(ComputerInfo computerInfo)
        {
            data.SaveComputerInfo(computerInfo);
        }

        public void SaveComputerLocation(string serialNumber, string myn, string byg, string lok, string owner) => data.SaveComputerLocation(serialNumber, myn, byg, lok, owner);

        public ComputerInfo GetComputerInfo(int id)
        {
            return data.GetComputerInfo(id);
        }

        public ComputerLocation GetComputerLocation(int id) => data.GetComputerLocation(id);

        public bool NeedsLocationUpdate(string serialNumber)
        {
            return data.LastLocationUpdate(serialNumber) == null;
        }

        public ComputerInfo GetComputerInfo(string serialNumber)
        {
            return data.GetComputerInfo(serialNumber);
        }

        public IEnumerable<ComputerInfo> GetComputerInfos()
        {
            return data.GetComputerInfos();
        }

        public ComputerInfo FindLastComputer(string username)
        {
            return data.FindLastComputer(username);
        }
    }
}