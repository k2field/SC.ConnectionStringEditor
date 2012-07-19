using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConnectionStringEditor
{
    /// <summary>
    /// Represents a connection string loader.
    /// </summary>
    public interface IConnectionStringLoader
    {
        /// <summary>
        /// Gets the name of the connection string.
        /// </summary>
        /// <value>
        /// The name of the connection string.
        /// </value>
        string ConnectionStringName { get; }

        /// <summary>
        /// Loads the connection string.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>The loaded connection string</returns>
        object LoadConnectionString(string connectionString);
    }
}
