using EA;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EADotnetAngularAddIn
{
    public partial class EntitesDialog : Form
    {

        public string[] SelectedItems => listEntities.SelectedItems.Cast<string>().ToArray();

        public EntitesDialog(EA.Repository repository)
        {
            InitializeComponent();
            listEntities.DataSource = repository.GetCurrentDiagram().DiagramObjects.Cast<DiagramObject>()
                        .Select(x => repository.GetElementByID(x.ElementID))
                        .Where(x => x.Stereotype == "Entity")
                        .ToList().Select(x => x.Name).ToArray();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}
