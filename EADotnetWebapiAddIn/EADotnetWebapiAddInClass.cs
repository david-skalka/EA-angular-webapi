using EA;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace EADotnetWebapiAddIn
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [Guid("D3D3D3D3-D3D3-D3D3-D3D3-D3D3D3D3D3D3")]
    public class EADotnetWebapiAddInClass
    {
        // Define menu constants
        const string menuHeader = "-&React Core";
        const string menuInitializeSolution = "&Initialize solution";
        const string menuGenerateDbContext = "&Generate db-context";
        const string menuGenerateSeeder = "&Generate seeder";
        const string menuGenerateEntities = "&Generate entities";
        const string menuSettings = "&Settings";

        public static string ConfigPath;

        SettingsService settingsService;

        public String EA_Connect(EA.Repository repository)
        {

            ConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EADotnetWebapiAddIn", "config.json");
            settingsService = new SettingsService(ConfigPath);
            settingsService.Load();

            return "a string";
        }




        public void EA_GetMenuState(Repository repository, string location, string menuName, string itemName, ref bool isEnabled, ref bool isChecked)
        {
            isEnabled = true;
        }







        public object EA_GetMenuItems(Repository repository, string location, string menuName)
        {
            switch (menuName)
            {
                case "":
                    return menuHeader;
                case menuHeader:
                    return new string[] { menuInitializeSolution, menuGenerateDbContext, menuGenerateSeeder, menuGenerateEntities, menuSettings };
            }
            return "";
        }




        void ExportXmi(EA.Repository repository, EA.Diagram diagram, string xmiPath)
        {
            repository.GetProjectInterface().ExportPackageXMI(repository.GetPackageByID(diagram.PackageID).PackageGUID, EnumXMIType.xmiEA21, 0, 0, 0, 0, xmiPath);
        }





        public void EA_MenuClick(EA.Repository repository, string Location, string MenuName, string ItemName)
        {
            
            

            var commands = new Dictionary<string, Action>
            {
                {
                    menuInitializeSolution,
                    () => ExecuteCli("initialize", new Dictionary<string, string>
                    {
                        { "-o", (string)settingsService.GetValue("output-dir")  },
                        { "-n", (string)settingsService.GetValue("project-name") }
                    })

                },
                { menuGenerateDbContext, () => {

                   var validationResult = ValidateDiagram(repository);

                    if (validationResult.Any())
                    {
                        DisplayValidationErrors(validationResult);
                        return;
                    }

                   var entityDialog = new EntitesDialog(repository);
                    entityDialog.ShowDialog();

                    if (entityDialog.DialogResult != DialogResult.OK)
                    {
                        return;
                    }

                    var xmiPath = Path.Combine(Path.GetTempPath(), @"react-core.xmi");
                    ExportXmi(repository, repository.GetCurrentDiagram(), xmiPath);

                    ExecuteCli("db-context", new Dictionary<string, string>
                    {
                        { "-o", (string)settingsService.GetValue("output-dir")  },
                        { "-e", string.Join(",", entityDialog.SelectedItems) },
                        { "-x", xmiPath },
                        { "-n",  (string)settingsService.GetValue("project-name") }
                    });
                }
                },
                { menuGenerateSeeder, () => {

                    var validationResult = ValidateDiagram(repository);

                    if (validationResult.Any())
                    {
                        DisplayValidationErrors(validationResult);
                        return;
                    }

                    var entityDialog = new EntitesDialog(repository);

                    entityDialog.ShowDialog();

                    if (entityDialog.DialogResult != DialogResult.OK)
                    {
                        return;
                    }

                    var xmiPath = Path.Combine(Path.GetTempPath(), @"react-core.xmi");
                    ExportXmi(repository, repository.GetCurrentDiagram(), xmiPath);

                    ExecuteCli("seeder", new Dictionary<string, string>
                    {
                        { "-o", (string)settingsService.GetValue("output-dir")  },
                        { "-e", string.Join(",", entityDialog.SelectedItems) },
                        { "-x", xmiPath },
                        { "-n",  (string)settingsService.GetValue("project-name") }
                    });
                }
                },
                { menuGenerateEntities, () => {

                    var validationResult = ValidateDiagram(repository);

                    if (validationResult.Any())
                    {
                        DisplayValidationErrors(validationResult);
                        return;
                    }

                    var entityDialog = new EntitesDialog(repository);
                    entityDialog.ShowDialog();

                    if (entityDialog.DialogResult != DialogResult.OK)
                    {
                        return;
                    }

                    var xmiPath = Path.Combine(Path.GetTempPath(), @"react-core.xmi");
                    ExportXmi(repository, repository.GetCurrentDiagram(), xmiPath);

                    ExecuteCli("entity", new Dictionary<string, string>
                    {
                        { "-o",  (string)settingsService.GetValue("output-dir") },
                        { "-e", string.Join(",", entityDialog.SelectedItems) },
                        { "-x", xmiPath },
                        { "-n", (string)settingsService.GetValue("project-name") }
                    });

                } },
                { menuSettings, () =>
                {
                    var settingsDialog = new SettingsDialogs(ConfigPath);
                    settingsDialog.ShowDialog();

                     if (settingsDialog.DialogResult != DialogResult.OK)
                    {
                        return;
                    }


                    settingsService.Load();
                }
                },

            };

            commands[ItemName]();
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


        private void DisplayValidationErrors(string[] message)
        {
            
            MessageBox.Show(string.Join("\n", message), "Validation errors", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        void ExecuteCli(string command, Dictionary<string, string> args)
        {
            

            var process = new Process();
            var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Sparx Systems\EAAddins64\EADotnetWebapiAddIn");

           process.StartInfo.FileName = (string)key.GetValue("CliInstallLocation");

            process.StartInfo.Arguments = command + " " + string.Join(" ", args.Select(x => x.Key + " \"" + x.Value + "\""));

            process.Start();
            process.WaitForExit();
        }


        ///
        /// EA calls this operation when it exists. Can be used to do some cleanup work.
        ///
        public void EA_Disconnect()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}