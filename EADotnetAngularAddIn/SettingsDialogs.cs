using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace EADotnetAngularAddIn
{
    public partial class SettingsDialogs : Form
    {
        string configPath;

        public SettingsDialogs(string configPath)
        {
            InitializeComponent();
            this.configPath = configPath;

        }

        private void SettingDialog_Load(object sender, EventArgs e)
        {
            this.txbContent.Text = File.ReadAllText(configPath);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            File.WriteAllText(configPath, this.txbContent.Text);
            Close();
        }
    }
}
