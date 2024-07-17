using EA;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace EADotnetAngularAddIn
{
    public partial class ExecuteDialog : Form
    {
        private readonly Repository repository;
        private readonly SettingsService settingsService;

        public string[] SelectedItems => listEntities.SelectedItems.Cast<string>().ToArray();

        public ExecuteDialog(EA.Repository repository, SettingsService settingsService)
        {
            InitializeComponent();
            this.repository = repository;
            this.settingsService = settingsService;
        }


        private string[] ValidateDiagram(Repository repository)
        {
            var retD = new List<string>();
            var selectedDiagram = repository.GetCurrentDiagram();
            if (selectedDiagram == null)
            {
                retD.Add("Please select a diagram");
            }

            return retD.ToArray();
        }




        private void btnExecute_Click(object sender, EventArgs e)
        {

            var validationResult = ValidateDiagram(repository);

            if (validationResult.Any())
            {
                MessageBox.Show(string.Join("\n", validationResult), "Validation errors", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            var xmiPath = Path.Combine(Path.GetTempPath(), @"model.xmi");
            repository.GetProjectInterface().ExportPackageXMI(repository.GetPackageByID(repository.GetCurrentDiagram().PackageID).PackageGUID, EnumXMIType.xmiEA21, 0, 0, 0, 0, xmiPath);

            var args = new Dictionary<string, string>
                    {
                        { "-p",  string.Join(",", listPart.SelectedItems.Cast<string>().ToArray()) },
                        { "-o",  (string)settingsService.GetValue("output-dir") },
                        { "-e", string.Join(",",listEntities.SelectedItems.Cast<string>().ToArray()) },
                        { "-x", xmiPath },
                        { "-n", (string)settingsService.GetValue("project-name") }
             };


            var process = new Process();
            var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Sparx Systems\EAAddins64\EADotnetAngularAddIn");

            process.StartInfo.FileName = (string)key.GetValue("CliInstallLocation");

            process.StartInfo.Arguments = string.Join(" ", args.Select(x => x.Key + " \"" + x.Value + "\""));

            process.Start();
            process.WaitForExit();



        }


      



        private void ExecuteDialog_Load(object sender, EventArgs e)
        {
            listEntities.DataSource = repository.GetCurrentDiagram().DiagramObjects.Cast<DiagramObject>()
                        .Select(x => repository.GetElementByID(x.ElementID))
                        .Where(x => x.Stereotype == "Entity")
                        .ToList().Select(x => x.Name).ToArray();



        }

        private void invertEntities_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listEntities.Items.Count; i++)
            {
                listEntities.SetSelected(i, !listEntities.GetSelected(i));
            }
        }

        private void invertParts_Click(object sender, EventArgs e)
        {
            // invert selection
            for (int i = 0; i < listPart.Items.Count; i++)
            {
                listPart.SetSelected(i, !listPart.GetSelected(i));
            }
        }
    }
}
