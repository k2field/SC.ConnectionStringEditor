using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConnectionStringEditor
{
    /// <summary>
    /// Represents a friendly exception.
    /// </summary>
    public class FriendlyException : Exception
    {
        /// <summary>
        /// Gets a value indicating whether this <see cref="FriendlyException"/> is fatal.
        /// </summary>
        /// <value>
        ///   <c>true</c> if fatal; otherwise, <c>false</c>.
        /// </value>
        public bool Fatal
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the title.
        /// </summary>
        public string Title
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FriendlyException"/> class.
        /// </summary>
        public FriendlyException(bool fatal, string title, string message)
            : base (message)
        {
            Title = title;
            Fatal = fatal;
        }

        public static void ThrowFatal(string title, string message)
        {
            throw new FriendlyException(true, title, message);
        }

        public static void ThrowMessage(string title, string message)
        {
            throw new FriendlyException(false, title, message);
        }
    }
}
