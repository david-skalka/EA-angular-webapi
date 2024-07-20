using EA;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace EADotnetAngularAddIn
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [Guid("D3D3D3D3-D3D3-D3D3-D3D3-D3D3D3D3D3D3")]
    public class EADotnetAngularAddInClass
    {
        // Define menu constants
        const string menuHeader = "-&Dotnet Angular";
        const string menuExecuteGenerator = "&Execute generator";
        const string menuShowSettings = "&Show settings";

        public static string ConfigPath;

        SettingsService settingsService;

        public String EA_Connect(EA.Repository repository)
        {

            ConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EADotnetAngularAddIn", "config.json");
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
                    return new string[] { menuExecuteGenerator, menuShowSettings };
            }
            return "";
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







        public void EA_MenuClick(EA.Repository repository, string Location, string MenuName, string ItemName)
        {

            var commands = new Dictionary<string, Action>
            {

                { menuExecuteGenerator, () =>
                {

                    var validationResult = ValidateDiagram(repository);

                    if (validationResult.Any())
                    {
                        MessageBox.Show(string.Join("\n", validationResult), "Validation errors", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    var xmiPath = Path.Combine(Path.GetTempPath(), @"ea-dotnet-angular-model.xmi");
                    repository.GetProjectInterface().ExportPackageXMI(repository.GetPackageByID(repository.GetCurrentDiagram().PackageID).PackageGUID, EnumXMIType.xmiEA21, 0, 0, 0, 0, xmiPath);

                    var args = new Dictionary<string, string>
                            {
                                { "-d",  (string)settingsService.GetValue("output-dir") },
                                { "-x", xmiPath },
                                { "-n", (string)settingsService.GetValue("project-name") },
                     };


                    var process = new Process();
                    var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Sparx Systems\EAAddins64\EADotnetAngularAddIn");

                    process.StartInfo.FileName = (string)key.GetValue("CliInstallLocation");
                    process.StartInfo.Arguments = string.Join(" ", args.Select(x => x.Key + " \"" + x.Value + "\"")) ;


                    process.StartInfo.UseShellExecute = true;
                    process.Start();
                    process.WaitForExit();

                }
                },

                { menuShowSettings, () =>
                {
                    var settingsDialog = new SettingsDialogs(ConfigPath);
                    settingsDialog.ShowDialog();

                    settingsService.Load();
                }
                },

            };

            commands[ItemName]();


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