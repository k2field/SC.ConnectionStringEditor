using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SourceCode;
using System.ComponentModel;
using System.Reflection;

namespace ConnectionStringEditor
{
    public class UrlPropertyDescriptor : PropertyDescriptor
    {
        private Type _componentType;
        public override Type ComponentType
        {
            get { return _componentType; }
        }

        private PropertyInfo _pi;

        public UrlPropertyDescriptor(Type componentType, string name, PropertyInfo pi)
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
            var v = _pi.GetValue(component, null);
            if (v == null)
                return null;
            return v.ToString();
        }

        public override bool IsReadOnly
        {
            get { return !_pi.CanWrite; }
        }

        public override Type PropertyType
        {
            get { return typeof(string); }
        }

        public override void ResetValue(object component)
        {

        }

        public override void SetValue(object component, object value)
        {
            if (value == null)
            {
                _pi.SetValue(component, null, null);
            }
            else
            {
                var sv = value.ToString();
                if (string.IsNullOrEmpty(sv))
                    _pi.SetValue(component, null, null);
                else
                    _pi.SetValue(component, new Uri(sv), null);
            }
        }

        public override bool ShouldSerializeValue(object component)
        {
            return true;
        }
    }

    /// <summary>
    /// Represents an EWS connection string builder.
    /// </summary>
    public class ExchangeWebServicesConnectionStringBuilder : ConnectionStringBuilderBase
    {
        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        [DisplayName("URL")]
        [Description("The URL for Exchange Web Services that will be used. This property will only have an effect if Autodiscover is false.")]
        [Category("Connection")]
        [RefreshProperties(RefreshProperties.All)]
        public Uri Url
        {
            get
            {
                var val = GetConvertedValue<string>("URL", null);
                if (string.IsNullOrEmpty(val))
                    return null;
                else
                    return new Uri(val, UriKind.Absolute);
            }
            set
            {
                if (value == null)
                    SetConvertedValue("URL", null, null);
                else
                    SetConvertedValue("URL", value.ToString(), null);
            }
        }

        /// <summary>
        /// Gets or sets the User ID.
        /// </summary>
        /// <value>
        /// The User ID.
        /// </value>
        [DisplayName("User ID")]
        [Description("The name of the user to use when connecting to Exchange Web Services.")]
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
        [Description("The password to use when connecting to Exchange Web Services.")]
        [Category("Connection")]
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
        /// Gets or sets the user to delegate.
        /// </summary>
        /// <value>
        /// The user to delegate.
        /// </value>
        [DisplayName("Delegate")]
        [Description("The user that should be used under delegation - if the account has rights to perform the delegation.")]
        [Category("Connection")]
        [RefreshProperties(RefreshProperties.All)]
        public string Delegate
        {
            get
            {
                return GetConvertedValue<string>("Delegate", null);
            }
            set
            {
                SetConvertedValue("Delegate", value, null);
            }
        }

        /// <summary>
        /// Gets or sets the poll interval.
        /// </summary>
        /// <value>
        /// The poll interval.
        /// </value>
        [DisplayName("Poll Interval")]
        [Description("The amount of time the server should wait between checking for new messages. The value may be suffixed with a unit ('S' for seconds, 'M' for minutes, 'H' for hours and 'D' for days).")]
        [Category("Behavior")]
        [RefreshProperties(RefreshProperties.All)]
        public string PollInterval
        {
            get
            {
                return GetConvertedValue<string>("Poll Interval", "30S");
            }
            set
            {
                value = value ?? "30S";
                SetConvertedValue("Poll Interval", value, "30S");
            }
        }

        /// <summary>
        /// Gets the poll interval time.
        /// </summary>
        [Browsable(false)]
        public TimeSpan PollIntervalTime
        {
            get
            {
                var suffix = 'S';
                var v = PollInterval;
                if (!char.IsDigit(v[v.Length - 1]))
                {
                    suffix = v[v.Length - 1];
                    v = v.Substring(0, v.Length - 1);
                }
                var d = double.Parse(v);
                switch (suffix)
                {
                    case 'S':
                        return TimeSpan.FromSeconds(d);
                    case 'M':
                        return TimeSpan.FromMinutes(d);
                    case 'H':
                        return TimeSpan.FromHours(d);
                    case 'D':
                        return TimeSpan.FromDays(d);
                    default:
                        throw new ArgumentOutOfRangeException("Poll Interval");
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="ExchangeWebServicesConnectionStringBuilder"/> is autodiscover.
        /// </summary>
        /// <value>
        /// 	<see langword="true"/> if autodiscover; otherwise, <see langword="false"/>.
        /// </value>
        [Description("Whether the server should use Autodiscover to find the Exchange Web Services endpoint.")]
        [Category("Connection")]
        [RefreshProperties(RefreshProperties.All)]
        public bool Autodiscover
        {
            get
            {
                return GetConvertedValue<bool>("Autodiscover", false);
            }
            set
            {
                SetConvertedValue("Autodiscover", value, false);
            }
        }

        protected override void GetProperties(System.Collections.Hashtable propertyDescriptors)
        {
            base.GetProperties(propertyDescriptors);
            propertyDescriptors["URL"] = new UrlPropertyDescriptor(typeof(ExchangeWebServicesConnectionStringBuilder), "URL", typeof(ExchangeWebServicesConnectionStringBuilder).GetProperty("Url", BindingFlags.Public | BindingFlags.Instance));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExchangeWebServicesConnectionStringBuilder"/> class.
        /// </summary>
        public ExchangeWebServicesConnectionStringBuilder()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExchangeWebServicesConnectionStringBuilder"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public ExchangeWebServicesConnectionStringBuilder(string connectionString)
            : base(connectionString)
        {

        }

        /// <summary>
        /// Gets the keywords.
        /// </summary>
        /// <returns>The keywords.</returns>
        protected override IEnumerable<string> GetKeywords()
        {
            yield return "Poll Interval";
            yield return "User ID";
            yield return "Password";
            yield return "Delegate";
            yield return "URL";
            yield return "Autodiscover";
        }
    }
}
