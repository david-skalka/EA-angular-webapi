using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace EADotnetWebapiAddIn
{
    public partial class SettingsDialogs : Form
    {
        private readonly string path;

        public SettingsDialogs(string path)
        {
            InitializeComponent();
            this.path = path;
        }

        private void SettingDialog_Load(object sender, EventArgs e)
        {
            var content = File.ReadAllText(path);
            this.txbContent.Text = content;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            File.WriteAllText(path, this.txbContent.Text);
            Close();
        }
    }
}
