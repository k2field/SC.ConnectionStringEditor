using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ConnectionStringEditor
{
    public partial class NewConnectionForm : Form
    {
        public string TargetEntity
        {
            get
            {
                return entityTextBox.Text;
            }
            set
            {
                entityTextBox.Text = value;
            }
        }

        public NewConnectionForm()
        {
            InitializeComponent();
        }

        private void NewConnectionForm_Load(object sender, EventArgs e)
        {

        }
    }
}
