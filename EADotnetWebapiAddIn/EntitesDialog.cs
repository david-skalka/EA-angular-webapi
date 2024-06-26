using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EADotnetWebapiAddIn
{
    public partial class EntitesDialog : Form
    {

        public string[] SelectedItems => listEntities.SelectedItems.Cast<string>().ToArray();

        public EntitesDialog(string[] entities)
        {
            InitializeComponent();
            listEntities.DataSource = entities;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}
