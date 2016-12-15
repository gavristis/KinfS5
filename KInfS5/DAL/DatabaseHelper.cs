using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;

namespace KInfS5.DAL
{
    public class DatabaseHelper
    {
        private readonly string _connection;

        public DatabaseHelper()
        {
            _connection = ConfigurationManager.AppSettings["connection"];
        }

        public DbConnection Connection
        {
            get { return new OracleConnection(_connection); }
        }

        public DbParameter CreateParameter(string name, string value)
        {
            var parameter = new OracleParameter(name, OracleDbType.NVarchar2, ParameterDirection.Input)
            {
                Value = value
            };
            return parameter;
        }
    }
}
