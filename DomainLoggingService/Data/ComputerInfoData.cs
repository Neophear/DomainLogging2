using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using DomainLoggingService.Model;
using DomainLoggingService.Models.Exceptions;

namespace DomainLoggingService.Data
{
    public class ComputerInfoData : IComputerInfoData
    {
        public void SaveComputerInfo(ComputerInfo computerInfo)
        {
            try
            {
                DataAccessLayer dal = new DataAccessLayer();

                dal.AddParameter("@SerialNumber", computerInfo.SerialNumber, SqlDbType.VarChar);
                dal.AddParameter("@Name", computerInfo.Name, SqlDbType.VarChar);
                dal.AddParameter("@Model", computerInfo.Model, SqlDbType.VarChar);
                dal.AddParameter("@OS", computerInfo.OS, SqlDbType.VarChar);
                dal.AddParameter("@CPU", computerInfo.CPU, SqlDbType.VarChar);
                dal.AddParameter("@CPUCores", computerInfo.CPUCores, SqlDbType.TinyInt);
                dal.AddParameter("@RAMGB", computerInfo.RAMGB, SqlDbType.Int);
                dal.AddParameter("@DiskTypeId", computerInfo.DiskMediaType, SqlDbType.TinyInt);
                dal.AddParameter("@DiskSize", computerInfo.DiskSize, SqlDbType.Int);
                dal.AddParameter("@GFX", computerInfo.GFX, SqlDbType.VarChar);
                dal.AddParameter("@TeamViewerId", computerInfo.TeamViewerId, SqlDbType.VarChar);
                dal.ExecuteStoredProcedure("SaveComputerInfo");
                dal.ClearParameters();
            }
            catch (Exception ex)
            {
                if (ex is SqlException)
                    throw new DataAccessException(ex.Message);
                else
                    throw ex;
            }
        }

        public DateTime? LastLocationUpdate(string serialNumber)
        {
            try
            {
                DataAccessLayer dal = new DataAccessLayer();

                dal.AddParameter("@SerialNumber", serialNumber, SqlDbType.VarChar);
                object objLastUpdate = dal.ExecuteScalar("SELECT [LastUpdate] FROM [ComputerLocation] WHERE [Id] = (SELECT TOP 1 [Id] FROM [ComputerInfo] WHERE [SerialNumber] = UPPER(@SerialNumber))");
                dal.ClearParameters();

                return objLastUpdate == DBNull.Value ? null : (DateTime?)objLastUpdate;
            }
            catch (Exception ex)
            {
                if (ex is SqlException)
                    throw new DataAccessException(ex.Message);
                else
                    throw ex;
            }
        }

        public void SaveComputerLocation(string serialNumber, string myn, string byg, string lok, string owner)
        {
            try
            {
                DataAccessLayer dal = new DataAccessLayer();

                dal.AddParameter("@SerialNumber", serialNumber, SqlDbType.VarChar);
                dal.AddParameter("@MYN", myn, SqlDbType.NVarChar);
                dal.AddParameter("@BYG", byg, SqlDbType.NVarChar);
                dal.AddParameter("@LOK", lok, SqlDbType.NVarChar);
                dal.AddParameter("@Owner", owner, SqlDbType.NVarChar);
                dal.ExecuteStoredProcedure("SaveComputerLocation");
                dal.ClearParameters();
            }
            catch (Exception ex)
            {
                if (ex is SqlException)
                    throw new DataAccessException(ex.Message);
                else
                    throw ex;
            }
        }

        public ComputerInfo FindLastComputer(string username)
        {
            try
            {
                DataAccessLayer dal = new DataAccessLayer();

                dal.AddParameter("@Username", username ?? "", SqlDbType.VarChar);
                DataTable dt = dal.ExecuteDataTable("SELECT TOP 1 * FROM [vwLogWithComputers] WHERE [Username] = UPPER(@Username) ORDER BY [Id] DESC");
                dal.ClearParameters();

                return dt.Rows.Count == 1 ? Map(dt.Rows[0]) : null;
            }
            catch (Exception ex)
            {
                if (ex is SqlException)
                    throw new DataAccessException(ex.Message);
                else
                    throw ex;
            }
        }

        public ComputerInfo GetComputerInfo(int id)
        {
            try
            {
                DataAccessLayer dal = new DataAccessLayer();

                dal.AddParameter("@Id", id, SqlDbType.Int);
                DataTable dt = dal.ExecuteDataTable("SELECT TOP 1 * FROM [vwComputerInfo] WHERE [Computer_Id] = @Id");
                dal.ClearParameters();

                return dt.Rows.Count == 1 ? Map(dt.Rows[0]) : null;
            }
            catch (Exception ex)
            {
                if (ex is SqlException)
                    throw new DataAccessException(ex.Message);
                else
                    throw ex;
            }
        }

        public ComputerLocation GetComputerLocation(int id)
        {
            try
            {
                DataAccessLayer dal = new DataAccessLayer();

                dal.AddParameter("@Id", id, SqlDbType.Int);
                DataTable dt = dal.ExecuteDataTable("SELECT TOP 1 * FROM [ComputerLocation] WHERE [Id] = @Id");
                dal.ClearParameters();

                return dt.Rows.Count == 1 ? new ComputerLocation
                {
                    MYN = dt.Rows[0]["MYN"].ToString(),
                    BYG = dt.Rows[0]["BYG"].ToString(),
                    LOK = dt.Rows[0]["LOK"].ToString(),
                    Owner = dt.Rows[0]["Owner"].ToString(),
                    LastUpdate = dt.Rows[0]["LastUpdate"] == DBNull.Value ? null : (DateTime?)dt.Rows[0]["LastUpdate"]
                } : null;
            }
            catch (Exception ex)
            {
                if (ex is SqlException)
                    throw new DataAccessException(ex.Message);
                else
                    throw ex;
            }
        }

        public ComputerInfo GetComputerInfo(string serialNumber)
        {
            try
            {
                DataAccessLayer dal = new DataAccessLayer();

                dal.AddParameter("@SerialNumber", serialNumber ?? "", SqlDbType.VarChar);
                DataTable dt = dal.ExecuteDataTable("SELECT TOP 1 * FROM [vwComputerInfo] WHERE [SerialNumber] = UPPER(@SerialNumber)");
                dal.ClearParameters();

                return dt.Rows.Count == 1 ? Map(dt.Rows[0]) : null;
            }
            catch (Exception ex)
            {
                if (ex is SqlException)
                    throw new DataAccessException(ex.Message);
                else
                    throw ex;
            }
        }

        public IEnumerable<ComputerInfo> GetComputerInfos()
        {
            try
            {
                return new DataAccessLayer()
                    .ExecuteDataTable("SELECT * FROM [vwComputerInfo]")
                    .AsEnumerable()
                    .Select(r => Map(r));
            }
            catch (Exception ex)
            {
                if (ex is SqlException)
                    throw new DataAccessException(ex.Message);
                else
                    throw ex;
            }
        }

        private ComputerInfo Map(DataRow row)
        {
            return new ComputerInfo
            {
                Id = (int)row["Computer_Id"],
                SerialNumber = row["SerialNumber"].ToString(),
                Name = row["Name"]?.ToString(),
                Model = row["Model"]?.ToString(),
                OS = row["OS"]?.ToString(),
                CPU = row["CPU"]?.ToString(),
                CPUCores = Convert.ToUInt16(row["CPUCores"] == DBNull.Value ? 0 : row["CPUCores"]),
                RAMGB = Convert.ToUInt32(row["RAMGB"] == DBNull.Value ? 0 : row["RAMGB"] ?? 0),
                DiskMediaType = (DiskType)Convert.ToInt32(row["DiskType_Id"] == DBNull.Value ? 1 : row["DiskType_Id"]),
                DiskSize = Convert.ToUInt32(row["DiskSize"] == DBNull.Value ? 0 : row["DiskSize"]),
                GFX = row["GFX"]?.ToString(),
                TeamViewerId = row["TeamViewerId"]?.ToString(),
                LastAlive = row["LastAlive"] == DBNull.Value ? null : (DateTime?)row["LastAlive"]
            };
        }
    }

    public interface IComputerInfoData
    {
        void SaveComputerInfo(ComputerInfo computerInfo);
        void SaveComputerLocation(string serialNumber, string myn, string byg, string lok, string owner);
        ComputerInfo GetComputerInfo(int id);
        ComputerInfo GetComputerInfo(string serialNumber);
        ComputerLocation GetComputerLocation(int id);
        ComputerInfo FindLastComputer(string username);
        IEnumerable<ComputerInfo> GetComputerInfos();
        DateTime? LastLocationUpdate(string serialNumber);
    }
}