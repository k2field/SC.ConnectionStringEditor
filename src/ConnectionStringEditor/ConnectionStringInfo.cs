using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConnectionStringEditor
{
    /// <summary>
    /// Represents connection string information.
    /// </summary>
    public class ConnectionStringInfo
    {
        /// <summary>
        /// Gets or sets the target entity.
        /// </summary>
        public string TargetEntity
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the name of the connection string.
        /// </summary>
        /// <value>
        /// The name of the connection string.
        /// </value>
        public string ConnectionStringName
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        public object ConnectionString
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionStringInfo"/> class.
        /// </summary>
        public ConnectionStringInfo(string targetEntity, string connectionStringName, object connectionString)
        {
            TargetEntity = targetEntity;
            ConnectionStringName = connectionStringName;
            ConnectionString = connectionString;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(TargetEntity))
                return ConnectionStringName;
            else
                return string.Format("{0} ({1})", ConnectionStringName, TargetEntity);
        }

        public string ConfigurationName
        {
            get
            {
                if (string.IsNullOrEmpty(TargetEntity))
                    return ConnectionStringName;
                else
                    return string.Format("{0}:{1}", ConnectionStringName, TargetEntity);
            }
        }
    }
}
