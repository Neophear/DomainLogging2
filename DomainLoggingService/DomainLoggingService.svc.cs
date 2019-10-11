using DomainLoggingService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace DomainLoggingService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class DomainLoggingService : IDomainLoggingService
    {
        /// <summary>
        /// Writes to the Log and returns boolean representing whether ComputerInfo is outdated
        /// </summary>
        /// <param name="serialNumber"></param>
        /// <param name="username"></param>
        /// <param name="parameters"></param>
        /// <returns>True if info is outdated and needs to be updated</returns>
        public bool WriteLog(string serialNumber, string username, string parameters)
        {
            return new Controller.LogController().WriteLog(serialNumber, username, parameters);
        }

        /// <summary>
        /// Updates ComputerInfo i database
        /// </summary>
        /// <param name="computerInfo"></param>
        public void SaveComputerInfo(ComputerInfo computerInfo)
        {
            new Controller.ComputerInfoController().SaveComputerInfo(computerInfo);
        }

        public void WriteErrorLog(string computerName, string message)
        {
            new Controller.LogController().WriteErrorLog(computerName, message);
        }

        public void SaveComputerLocation(string serialNumber, string myn, string byg, string lok, string owner)
        {
            new Controller.ComputerInfoController().SaveComputerLocation(serialNumber, myn, byg, lok, owner);
        }

        public bool NeedsLocationUpdate(string serialNumber)
        {
            return new Controller.ComputerInfoController().NeedsLocationUpdate(serialNumber);
        }
    }

    [ServiceContract]
    public interface IDomainLoggingService
    {
        [OperationContract]
        bool WriteLog(string serialNumber, string username, string parameters);

        [OperationContract]
        void SaveComputerInfo(ComputerInfo computerInfo);

        [OperationContract]
        void SaveComputerLocation(string serialNumber, string myn, string byg, string lok, string owner);

        [OperationContract]
        void WriteErrorLog(string computerName, string message);

        [OperationContract]
        bool NeedsLocationUpdate(string serialNumber);
    }
}
