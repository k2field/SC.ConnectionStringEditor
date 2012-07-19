using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SourceCode;
using System.ComponentModel;
using System.Reflection;

namespace ConnectionStringEditor
{
    public class EnumPropertyDescriptor<TEnum> : PropertyDescriptor
    {
        private Type _componentType;
        public override Type ComponentType
        {
            get { return _componentType; }
        }

        private PropertyInfo _pi;

        public EnumPropertyDescriptor(Type componentType, string name, PropertyInfo pi)
            : base(name, pi.GetCustomAttributes(true).Cast<Attribute>().ToArray())
        {
            _pi = pi;
            _componentType = componentType;
        }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override object GetValue(object component)
        {
            return _pi.GetValue(component, null);
        }

        public override bool IsReadOnly
        {
            get { return !_pi.CanWrite; }
        }

        public override Type PropertyType
        {
            get { return typeof(TEnum); }
        }

        public override void ResetValue(object component)
        {

        }

        public override void SetValue(object component, object value)
        {
            _pi.SetValue(component, value, null);
        }

        public override bool ShouldSerializeValue(object component)
        {
            return true;
        }
    }

    /// <summary>
    /// Represents an SMTP connection string.
    /// </summary>
    public class SmtpConnectionStringBuilder : ConnectionStringBuilderBase
    {
        #region Connection Properties
        /// <summary>
        /// Gets or sets the host to connect to.
        /// </summary>
        [DisplayName("Host Name")]
        [Description("The name of the machine to connect to when sending an SMTP message.")]
        [Category("Connection")]
        [RefreshProperties(RefreshProperties.All)]
        public string Host
        {
            get
            {
                return GetConvertedValue<string>("Host Name", null);
            }
            set
            {
                SetConvertedValue("Host Name", value, null);
            }
        }

        /// <summary>
        /// Gets or sets the TLS [Transport Layer Security] mode to use.
        /// </summary>
        [DisplayName("TLS")]
        [Description("The TLS/SSL mode to use when connecting to the host.")]
        [Category("Connection")]
        [RefreshProperties(RefreshProperties.All)]
        public SourceCode.Net.TlsMode TlsMode
        {
            get
            {
                return GetConvertedValue<SourceCode.Net.TlsMode>("TLS", SourceCode.Net.TlsMode.StartTls);
            }
            set
            {
                SetConvertedValue("TLS", value, null);
            }
        }

        /// <summary>
        /// Gets or sets the port to connect to.
        /// </summary>
        [DisplayName("Port")]
        [Description("The port to use when connecting to the host.")]
        [Category("Connection")]
        [RefreshProperties(RefreshProperties.All)]
        public int Port
        {
            get
            {
                return GetConvertedValue<int>("Port", GetDefaultPort(TlsMode));
            }
            set
            {
                SetConvertedValue("Port", value, null);
            }
        }
        #endregion

        /// <summary>
        /// Gets or sets the user ID.
        /// </summary>
        /// <value>
        /// The user ID.
        /// </value>
        [DisplayName("User ID")]
        [Description("The name of the user to use to authenticate with the host.")]
        [Category("Connection")]
        [RefreshProperties(RefreshProperties.All)]
        public string UserID
        {
            get
            {
                return GetConvertedValue<string>("User ID", null);
            }
            set
            {
                SetConvertedValue("User ID", value, null);
            }
        }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        [DisplayName("Password")]
        [Description("The password for the user to use when connecting to the host.")]
        [Category("Connection")]
        [RefreshProperties(RefreshProperties.All)]
        public string Password
        {
            get
            {
                return GetConvertedValue<string>("Password", null);
            }
            set
            {
                SetConvertedValue("Password", value, null);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to use integrated authentication.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if integrated authentication should be used; otherwise, <c>false</c>.
        /// </value>
        [DisplayName("Authentication")]
        [Description("The authentication mechanism to use when connecting to the host.")]
        [Category("Connection")]
        [RefreshProperties(RefreshProperties.All)]
        public SourceCode.Net.Mail.SmtpAuthenticationType AuthenticationType
        {
            get
            {
                return (SourceCode.Net.Mail.SmtpAuthenticationType)Enum.Parse(typeof(SourceCode.Net.Mail.SmtpAuthenticationType), GetConvertedValue<string>("Authentication", "Anonymous"), true);
            }
            set
            {
                SetConvertedValue("Authentication", value.ToString(), "Anonymous");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SmtpConnectionString"/> class.
        /// </summary>
        public SmtpConnectionStringBuilder()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SmtpConnectionString"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public SmtpConnectionStringBuilder(string connectionString)
            : base(connectionString)
        {

        }


        protected override void GetProperties(System.Collections.Hashtable propertyDescriptors)
        {
            base.GetProperties(propertyDescriptors);
//            var noCustom = TypeDescriptor.GetProperties(this, false);
            propertyDescriptors["TLS"] = new EnumPropertyDescriptor<SourceCode.Net.TlsMode>(typeof(SmtpConnectionStringBuilder), "TLS", typeof(SmtpConnectionStringBuilder).GetProperty("TlsMode", BindingFlags.Public | BindingFlags.Instance));
            propertyDescriptors["Authentication"] = new EnumPropertyDescriptor<SourceCode.Net.Mail.SmtpAuthenticationType>(typeof(SmtpConnectionStringBuilder), "Authentication", typeof(SmtpConnectionStringBuilder).GetProperty("AuthenticationType", BindingFlags.Public | BindingFlags.Instance));
        }

        /// <summary>
        /// Gets the supported keywords for the <see cref="SocketConnectionStringBuilder"/>.
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<string> GetKeywords()
        {
            yield return "Port";
            yield return "TLS";
            yield return "Host Name";
            yield return "User ID";
            yield return "Password";
            yield return "Authentication";
        }

        /// <summary>
        /// Gets the default port for the specified TLS mode.
        /// </summary>
        /// <param name="tlsMode">The TLS mode.</param>
        /// <returns>The default port.</returns>
        protected int GetDefaultPort(SourceCode.Net.TlsMode tlsMode)
        {
            switch (tlsMode)
            {
                case SourceCode.Net.TlsMode.BootTls:
                    return 587;
                default:
                    return 25;
            }
        }
    }
}
