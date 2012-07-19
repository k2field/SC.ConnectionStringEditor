using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConnectionStringEditor
{
    public class SmtpConnectionStringLoader : IConnectionStringLoader
    {
        public string ConnectionStringName
        {
            get { return "SourceCode.Net.Mail.SmtpConnection"; }
        }

        public object LoadConnectionString(string connectionString)
        {
            return new SmtpConnectionStringBuilder(connectionString);
        }
    }
}
