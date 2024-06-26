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
        const string menuGenerateEntities = "&Generate entities";
        const string menuSettings = "&Settings";

        SettingsService settingsService = new SettingsService() { ConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EADotnetWebapiAddIn", "config.json") };

        public String EA_Connect(EA.Repository repository)
        {
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
                    return new string[] { menuInitializeSolution, menuGenerateDbContext, menuGenerateEntities, menuSettings };
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
                 var selectedDiagram = repository.GetCurrentDiagram();


                     var entities = selectedDiagram.DiagramObjects.Cast<DiagramObject>()
                        .Select(x => repository.GetElementByID(x.ElementID))
                        .Where(x => x.Stereotype == "Entity")
                        .ToList().Select(x => x.Name).ToArray();

                        var entityDialog = new EntitesDialog(entities);

                    entityDialog.ShowDialog();


                    var xmiPath = Path.Combine(Path.GetTempPath(), @"react-core.xmi");
                    ExportXmi(repository, selectedDiagram, xmiPath);

                    ExecuteCli("db-context", new Dictionary<string, string>
                    {
                        { "-o", (string)settingsService.GetValue("output-dir")  },
                        { "-e", string.Join(",", entityDialog.SelectedItems) },
                        { "-x", xmiPath },
                        { "-n",  (string)settingsService.GetValue("project-name") }
                    });
                }
                },
                { menuGenerateEntities, () => {


                    var selectedDiagram = repository.GetCurrentDiagram();

                    var entities = selectedDiagram.DiagramObjects.Cast<DiagramObject>()
                        .Select(x => repository.GetElementByID(x.ElementID))
                        .Where(x => x.Stereotype == "Entity")
                        .ToList().Select(x => x.Name).ToArray();


                    var entityDialog = new EntitesDialog(entities);
                    entityDialog.ShowDialog();

                    var xmiPath = Path.Combine(Path.GetTempPath(), @"react-core.xmi");
                    ExportXmi(repository, selectedDiagram, xmiPath);





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
                    var settingsDialog = new SettingsDialogs(settingsService.ConfigPath);
                    settingsDialog.ShowDialog();
                    settingsService.Load();
                }
                },

            };

            commands[ItemName]();
        }


        void ExecuteCli(string command, Dictionary<string, string> args)
        {
            

            var process = new Process();
            process.StartInfo.FileName = (string)settingsService.GetValue("cli-path.override");

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