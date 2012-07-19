using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace ConnectionStringEditor
{
    public class SqlConnectionStringLoader : IConnectionStringLoader
    {
        public string ConnectionStringName
        {
            get { return "HostServerDB"; }
        }

        public object LoadConnectionString(string connectionString)
        {
            return new SqlConnectionStringBuilder(connectionString);
        }
    }
}
