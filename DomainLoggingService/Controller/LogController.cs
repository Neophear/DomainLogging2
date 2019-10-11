using DomainLoggingService.Data;
using DomainLoggingService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DomainLoggingService.Controller
{
    public class LogController
    {
        readonly ILogData data;

        public LogController(ILogData data = null)
        {
            this.data = data ?? new LogData();
        }

        public bool WriteLog(string serialNumber, string username, string parameters)
        {
            DateTime? lastUpdate = data.WriteLog(serialNumber, username, parameters);

            return (lastUpdate ?? DateTime.MinValue) < DateTime.Now.AddDays(-30);
        }

        public void WriteErrorLog(string computerName, string message)
        {
            data.WriteErrorLog(computerName, message);
        }

        public IEnumerable<LogEntry> GetLog(int topRecords)
        {
            if (topRecords < 1)
                topRecords = 1;
            else if (topRecords > 500)
                topRecords = 500;

            return data.GetLog(topRecords);
        }

        public IEnumerable<LogEntry> GetComputerLog(int computerId)
        {
            return data.GetComputerLog(computerId);
        }

        public IEnumerable<LogEntry> GetUserLog(string username)
        {
            return data.GetUserLog(username);
        }
    }
}