using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLoggingService.Data
{
    public class DataAccessLayer
    {
        private SqlConnection Conn;

        private List<SqlParameter> Parameters;

        public void AddParameter(string name, object value, SqlDbType type, ParameterDirection direction = ParameterDirection.Input)
        {
            SqlParameter p = new SqlParameter(name, value);
            p.SqlDbType = type;
            p.Direction = direction;
            Parameters.Add(p);
        }

        public SqlParameter GetParameter(string name)
        {
            return Parameters.Find(x => x.ParameterName == name);
        }

        public void ClearParameters()
        {
            Parameters.Clear();
        }
        public DataAccessLayer(string connectionString = null)
        {
            if (connectionString == null)
                connectionString = ConfigurationManager.ConnectionStrings["DLDB2"].ConnectionString;

            Conn = new SqlConnection(connectionString);
            Parameters = new List<SqlParameter>();
        }
        public DataTable ExecuteDataTable(string SQL)
        {
            DataTable dt;

            using (SqlCommand Comm = Conn.CreateCommand())
            {
                Comm.CommandText = SQL;

                if (Parameters.Count > 0)
                {
                    Comm.Parameters.AddRange(Parameters.ToArray());
                }
                SqlDataAdapter da = new SqlDataAdapter(Comm);
                dt = new DataTable();
                da.Fill(dt);
            }

            return dt;
        }
        public SqlDataReader ExecuteReader(string SQL)
        {
            SqlCommand Comm = Conn.CreateCommand();
            Comm.CommandText = SQL;
            Comm.CommandType = CommandType.Text;

            if (Parameters.Count > 0)
            {
                Comm.Parameters.AddRange(Parameters.ToArray());
            }

            Conn.Open();

            SqlDataReader Reader = Comm.ExecuteReader(CommandBehavior.CloseConnection);

            return Reader;
        }
        public int ExecuteNonQuery(string SQL)
        {
            using (SqlCommand Comm = Conn.CreateCommand())
            {
                Comm.CommandText = SQL;

                if (Parameters.Count > 0)
                {
                    Comm.Parameters.AddRange(Parameters.ToArray());
                }

                Conn.Open();
                int i = Comm.ExecuteNonQuery();
                Conn.Close();

                return i;
            }
        }
        public object ExecuteScalar(string SQL)
        {
            object result;
            using (SqlCommand Comm = Conn.CreateCommand())
            {
                Comm.CommandText = SQL;
                if (Parameters.Count > 0)
                {
                    Comm.Parameters.AddRange(Parameters.ToArray());
                }

                Conn.Open();
                result = Comm.ExecuteScalar();
                Conn.Close();
            }
            return result;
        }
        
        public void ExecuteStoredProcedure(string SQL)
        {
            using (SqlCommand Comm = Conn.CreateCommand())
            {
                Comm.CommandType = CommandType.StoredProcedure;
                Comm.CommandText = SQL;
                if (Parameters.Count > 0)
                {
                    Comm.Parameters.AddRange(Parameters.ToArray());
                }

                Conn.Open();
                Comm.ExecuteNonQuery();
                Conn.Close();
            }
        }
    }
}
