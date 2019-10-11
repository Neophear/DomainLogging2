using DomainLoggingService.Models.Exceptions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLoggingService.Data
{
    public class DataExceptionCheck
    {
        public static Exception CheckException(Exception e)
        {
            //Dont repack exceptions
            if (e.GetType().Equals(typeof(DataAccessException)) ||
                e.GetType().Equals(typeof(NotFoundException)) ||
                e.GetType().Equals(typeof(NotAuthorizedException)))
                return e;

            if (e.GetType().Equals(typeof(SqlException)))
            {
                switch (((SqlException)e).Number)
                {
                    case 50404:
                        return new NotFoundException(e.Message);
                    case 50403:
                        return new NotAuthorizedException(e.Message);
                    default:
                        return new DataAccessException(e.Message);
                }
            }
            else
                return new DataAccessException(e.Message);
        }
    }
}