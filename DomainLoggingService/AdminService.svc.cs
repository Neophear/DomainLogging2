using DomainLoggingService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;

namespace DomainLoggingService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "AdminService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select AdminService.svc or AdminService.svc.cs at the Solution Explorer and start debugging.
    public class AdminService : IAdminService
    {
        [OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
        public ComputerInfo GetComputerInfoFromId(int id)
        {
            return RunCode(() => new Controller.ComputerInfoController().GetComputerInfo(id));
        }

        [OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
        public ComputerLocation GetComputerLocation(int id)
        {
            return RunCode(() => new Controller.ComputerInfoController().GetComputerLocation(id));
        }

        [OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
        public ComputerInfo GetComputerInfoFromSerialNumber(string serialNumber)
        {
            return RunCode(() => new Controller.ComputerInfoController().GetComputerInfo(serialNumber));
        }

        [OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
        public IEnumerable<ComputerInfo> GetComputerInfos()
        {
            return RunCode(() => new Controller.ComputerInfoController().GetComputerInfos());
        }

        [OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
        public ComputerInfo FindLastComputer(string username)
        {
            return RunCode(() => new Controller.ComputerInfoController().FindLastComputer(username));
        }

        [OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
        public IEnumerable<LogEntry> GetLog(int topRecords)
        {
            return RunCode(() => new Controller.LogController().GetLog(topRecords));
        }

        [OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
        public IEnumerable<LogEntry> GetComputerLog(int computerId)
        {
            return RunCode(() => new Controller.LogController().GetComputerLog(computerId));
        }

        [OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
        public IEnumerable<LogEntry> GetUserLog(string username)
        {
            return RunCode(() => new Controller.LogController().GetUserLog(username));
        }

        [OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
        public User GetUser(string username)
        {
            return RunCode(() => new Controller.UserController().GetUser(username));
        }

        private T RunCode<T>(Func<T> f)
        {
            try
            {
                new PrincipalPermission(ServiceSecurityContext.Current.WindowsIdentity.Name, "Domain Admins", true).Demand();

                return f();
            }
            catch (System.Security.SecurityException)
            {
                throw new System.ServiceModel.Web.WebFaultException(System.Net.HttpStatusCode.Forbidden);
            }
            catch (Exception)
            {
                throw new System.ServiceModel.Web.WebFaultException(System.Net.HttpStatusCode.InternalServerError);
            }
        }
    }

    [ServiceContract]
    public interface IAdminService
    {
        [OperationContract]
        ComputerInfo GetComputerInfoFromId(int id);

        [OperationContract]
        ComputerInfo GetComputerInfoFromSerialNumber(string serialNumber);

        [OperationContract]
        ComputerLocation GetComputerLocation(int id);

        [OperationContract]
        IEnumerable<ComputerInfo> GetComputerInfos();

        [OperationContract]
        ComputerInfo FindLastComputer(string username);

        /// <summary>
        /// Returns recent log entries (max 500)
        /// </summary>
        /// <param name="topRecords"></param>
        /// <returns></returns>
        [OperationContract]
        IEnumerable<LogEntry> GetLog(int topRecords);

        [OperationContract]
        IEnumerable<LogEntry> GetComputerLog(int computerId);

        [OperationContract]
        IEnumerable<LogEntry> GetUserLog(string username);

        [OperationContract]
        User GetUser(string username);
    }
}
