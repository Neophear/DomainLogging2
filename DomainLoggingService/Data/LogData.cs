using DomainLoggingService.Model;
using DomainLoggingService.Models.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;

namespace DomainLoggingService.Data
{
    public class LogData : ILogData
    {
        public DateTime? WriteLog(string serialNumber, string username, string parameters)
        {
            try
            {
                DataAccessLayer dal = new DataAccessLayer();

                dal.AddParameter("@SerialNumber", serialNumber, SqlDbType.VarChar);
                dal.AddParameter("@Username", username, SqlDbType.VarChar);
                dal.AddParameter("@Parameters", parameters ?? "", SqlDbType.VarChar);
                dal.AddParameter("@LastUpdate", DBNull.Value, SqlDbType.DateTime, ParameterDirection.Output);
                dal.ExecuteStoredProcedure("WriteLog");
                object objLastUpdate = dal.GetParameter("@LastUpdate").Value;
                dal.ClearParameters();

                return objLastUpdate == DBNull.Value ? null : (DateTime?)objLastUpdate;
            }
            catch (Exception ex)
            {
                WriteErrorLog("SERVICE", ex.Message);

                if (ex is SqlException)
                    throw new DataAccessException(ex.Message);
                else
                    throw ex;
            }
        }

        public IEnumerable<LogEntry> GetUserLog(string username)
        {
            try
            {
                DataAccessLayer dal = new DataAccessLayer();

                dal.AddParameter("@username", username, SqlDbType.VarChar);
                DataTable dt = dal.ExecuteDataTable("SELECT TOP 500 [Id], [Timestamp], [Computer_Id], [Username], [Parameters], [SerialNumber] FROM [vwLogWithComputers] WHERE [Username] = UPPER(@username) ORDER BY [Id] DESC");
                dal.ClearParameters();

                return dt.AsEnumerable().Select(r => Map(r));
            }
            catch (Exception ex)
            {
                WriteErrorLog("SERVICE", ex.Message);

                if (ex is SqlException)
                    throw new DataAccessException(ex.Message);
                else
                    throw ex;
            }
        }

        public IEnumerable<LogEntry> GetLog(int topRecords)
        {
            try
            {
                DataAccessLayer dal = new DataAccessLayer();

                dal.AddParameter("@Top", topRecords, SqlDbType.Int);
                DataTable dt = dal.ExecuteDataTable("SELECT TOP (@Top) [Id], [Timestamp], [Computer_Id], [Username], [Parameters], [SerialNumber] FROM [vwLogWithComputers] ORDER BY [Id] DESC");
                dal.ClearParameters();

                return dt.AsEnumerable().Select(r => Map(r));
            }
            catch (Exception ex)
            {
                WriteErrorLog("SERVICE", ex.Message);

                if (ex is SqlException)
                    throw new DataAccessException(ex.Message);
                else
                    throw ex;
            }
        }

        public IEnumerable<LogEntry> GetComputerLog(int computerId)
        {
            try
            {
                DataAccessLayer dal = new DataAccessLayer();

                dal.AddParameter("@ComputerId", computerId, SqlDbType.Int);
                DataTable dt = dal.ExecuteDataTable("SELECT TOP 500 [Id], [Timestamp], [Username], [Parameters] FROM [Log] WHERE [Computer_Id] = @ComputerId ORDER BY [Id] DESC");
                dal.ClearParameters();

                return dt.AsEnumerable().Select(r => new LogEntry
                {
                    Id = (long)r["Id"],
                    Timestamp = (DateTime)r["Timestamp"],
                    Username = r["Username"].ToString(),
                    Parameters = r["Parameters"].ToString()
                });
            }
            catch (Exception ex)
            {
                WriteErrorLog("SERVICE", ex.Message);

                if (ex is SqlException)
                    throw new DataAccessException(ex.Message);
                else
                    throw ex;
            }
        }

        public void WriteErrorLog(string computerName, string message)
        {
            try
            {
                DataAccessLayer dal = new DataAccessLayer();

                dal.AddParameter("@ComputerName", computerName, SqlDbType.VarChar);
                dal.AddParameter("@Message", message.Length <= 1000 ? message : message.Substring(0, 1000), SqlDbType.VarChar);
                dal.ExecuteStoredProcedure("WriteErrorLog");
                dal.ClearParameters();
            }
            catch (Exception ex)
            {
                File.AppendAllText(@"L:\Websites\DLService\errors.txt", $"Computer: {computerName} - {DateTime.Now}\r\n{ex}\r\n\r\n");
                //File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\DLDBServiceErrors.txt", $"Computer: {computerName} - {DateTime.Now}\r\n{ex}\r\n\r\n");
            }
        }

        private LogEntry Map(DataRow row)
        {
            return new LogEntry
            {
                Id = (long)row["Id"],
                Timestamp = (DateTime)row["Timestamp"],
                Username = row["Username"].ToString(),
                Parameters = row["Parameters"].ToString(),
                ComputerId = (int)row["Computer_Id"],
                SerialNumber = row["SerialNumber"].ToString()
            };
        }
    }

    public interface ILogData
    {
        DateTime? WriteLog(string serialNumber, string username, string parameters);
        void WriteErrorLog(string computerName, string message);
        IEnumerable<LogEntry> GetLog(int topRecords);
        IEnumerable<LogEntry> GetComputerLog(int computerId);
        IEnumerable<LogEntry> GetUserLog(string username);
    }
}