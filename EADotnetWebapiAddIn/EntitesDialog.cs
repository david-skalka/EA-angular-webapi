using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace EADotnetWebapiAddIn
{
    public partial class EntitesDialog : Form
    {

        public string SelectedItem => (string)cmbEntities.SelectedItem;

        public EntitesDialog(string[] entities)
        {
            InitializeComponent();
            cmbEntities.DataSource = entities;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}
