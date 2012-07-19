using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using System.IO;

namespace ConnectionStringEditor
{
    public partial class MainForm : Form
    {
        private static readonly Dictionary<string, IConnectionStringLoader> _loaders = new Dictionary<string, IConnectionStringLoader>(StringComparer.OrdinalIgnoreCase);
        private HashSet<string> _deletedConnections = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private Configuration _configuration;
        private ConnectionStringsSection _connectionStrings;

        static MainForm()
        {
            AddLoader(new ExchangeWebServicesConnectionStringLoader());
            AddLoader(new SmtpConnectionStringLoader());
            AddLoader(new SqlConnectionStringLoader());
        }

        static void AddLoader(IConnectionStringLoader loader)
        {
            _loaders.Add(loader.ConnectionStringName, loader);
        }

        public MainForm()
        {
            InitializeComponent();
        }

        private bool _firstRun = true;
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            if (_firstRun)
            {
                _firstRun = false;
                try
                {
                    LoadConnectionStrings();
                }
                catch (FriendlyException ex)
                {
                    MessageBox.Show(ex.Message, ex.Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                }
            }
        }

        private void LoadConnectionStrings()
        {
            var map = new ExeConfigurationFileMap();
            map.ExeConfigFilename = FindConfig("K2HostServer.config", "K2HostServer.exe.config");

            _configuration = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
            _connectionStrings = _configuration.GetSection("connectionStrings") as ConnectionStringsSection;

            foreach (ConnectionStringSettings item in _connectionStrings.ConnectionStrings)
            {
                try
                {
                    var colonIndex = item.Name.IndexOf(':');
                    string connectionStringName = null, targetEntity = null;
                    if (colonIndex < 0)
                    {
                        connectionStringName = item.Name;
                    }
                    else
                    {
                        connectionStringName = item.Name.Substring(0, colonIndex);
                        targetEntity = item.Name.Substring(colonIndex + 1);
                    }

                    IConnectionStringLoader loader;
                    if (_loaders.TryGetValue(connectionStringName, out loader))
                    {
                        var connectionString = loader.LoadConnectionString(item.ConnectionString);
                        var info = new ConnectionStringInfo(targetEntity, connectionStringName, connectionString);
                        mainListBox.Items.Add(info);
                    }
                }
                catch (Exception ex)
                {
                    FriendlyException.ThrowFatal("Failure loading connection strings", ex.Message);
                }
            }
        }

        private string FindConfig(params string[] options)
        {
            foreach (var item in options)
            {
                var path = Path.GetFullPath(item);
                if (File.Exists(path))
                    return path;
            }
            FriendlyException.ThrowFatal(ConnectionStringEditor.Properties.Resources.NoConfigurationFile, ConnectionStringEditor.Properties.Resources.NoConfigurationFileHint);
            return null;
        }

        private void mainListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = mainListBox.SelectedItem as ConnectionStringInfo;
            if (item != null)
            {
                deleteToolStripButton.Enabled = !string.Equals(item.ConnectionStringName, "HostServerDB", StringComparison.OrdinalIgnoreCase);
                mainPropertyGrid.SelectedObject = item.ConnectionString;
            }
            else
            {
                deleteToolStripButton.Enabled = false;
                mainPropertyGrid.SelectedObject = null;
            }
        }

        private void deleteToolStripButton_Click(object sender, EventArgs e)
        {
            var item = mainListBox.SelectedItem as ConnectionStringInfo;
            if (item != null)
            {
                _deletedConnections.Add(item.ConfigurationName);
                mainListBox.Items.Remove(mainListBox.SelectedItem);
            }
        }

        private void exchangeWebServicesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddConnectionString("SourceCode.MessageBus.Ews.ExchangeWebServicesConnection", "Exchange Web Services Connection", false, new ExchangeWebServicesConnectionStringBuilder());
        }

        private void sMTPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddConnectionString("SourceCode.Net.Mail.SmtpConnection", "SMTP Connection", true, new SmtpConnectionStringBuilder());
        }

        private void AddConnectionString(string systemName, string title, bool allowEmpty, object connectionString)
        {
            using (var prompt = new NewConnectionForm())
            {
                prompt.Text = title;
                while (true)
                {
                    if (prompt.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                        return;
                    if (allowEmpty || !string.IsNullOrEmpty(prompt.TargetEntity))
                    {
                        var entityName = string.IsNullOrEmpty(prompt.TargetEntity) ? null : prompt.TargetEntity;
                        var found = false;
                        foreach (ConnectionStringInfo item in mainListBox.Items)
                        {
                            if (item.ConnectionStringName == systemName && string.Equals(item.TargetEntity, entityName, StringComparison.OrdinalIgnoreCase))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (found)
                        {
                            MessageBox.Show("Specified entity already exists with that connection type.", "Entity name already exists", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        }
                        else
                        {
                            mainListBox.Items.Add(new ConnectionStringInfo(entityName, systemName, connectionString));
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please provide an entity name.", "Entity name required", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                }
            }
        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            foreach (var item in _deletedConnections)
                _connectionStrings.ConnectionStrings.Remove(item);
            _deletedConnections.Clear();
            foreach (ConnectionStringInfo item in mainListBox.Items)
            {
                _connectionStrings.ConnectionStrings.Remove(item.ConfigurationName);
                _connectionStrings.ConnectionStrings.Add(new ConnectionStringSettings(item.ConfigurationName, item.ConnectionString.ToString()));
            }
            _configuration.Save();
        }
    }
}
