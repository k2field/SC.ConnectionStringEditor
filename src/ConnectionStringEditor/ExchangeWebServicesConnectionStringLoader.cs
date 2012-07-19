using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConnectionStringEditor
{
    public class ExchangeWebServicesConnectionStringLoader : IConnectionStringLoader
    {
        public string ConnectionStringName
        {
            get { return "SourceCode.MessageBus.Ews.ExchangeWebServicesConnection"; }
        }

        public object LoadConnectionString(string connectionString)
        {
            return new ExchangeWebServicesConnectionStringBuilder(connectionString);
        }
    }
}
