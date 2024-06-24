using EA;
using System;
using System.Collections.Generic;
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

        string configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EAReactCoreAddIn", "config.json");


        
        
        public String EA_Connect(EA.Repository repository)
        {
            
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

        public Dictionary<string,object> ReadConfig()
        {
            var content = System.IO.File.ReadAllText(configPath);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
        }


        void ExportXmi(EA.Repository repository, EA.Diagram diagram, string xmiPath)
        {
            repository.GetProjectInterface().ExportPackageXMI(repository.GetPackageByID(diagram.PackageID).PackageGUID, EnumXMIType.xmiEA21, 0, 0, 0, 0, xmiPath);
        }

        



        public void EA_MenuClick(EA.Repository repository, string Location, string MenuName, string ItemName)
        {



            switch (ItemName)
            {
                case menuInitializeSolution:
                    MessageBox.Show("Initialize solution");

                    break;
                case menuGenerateEntities:

                    var selectedDiagram = repository.GetCurrentDiagram();

                    var entities = selectedDiagram.DiagramObjects.Cast<DiagramObject>()
                        .Select(x => repository.GetElementByID(x.ElementID))
                        .Where(x => x.Stereotype == "Entity")
                        .ToList().Select(x => x.Name).ToArray();




                    var entityDialog = new EntitesDialog(entities);
                    entityDialog.ShowDialog();

                    var xmiPath = Path.Combine(Path.GetTempPath(), @"react-core.xmi");

                    ExportXmi(repository, selectedDiagram, xmiPath);
                    var config = ReadConfig();

                    var process = new System.Diagnostics.Process();
                    process.StartInfo.FileName = (string)config["cli-path.override"];
                    
                    process.StartInfo.Arguments = "entity -p \"" + config["project-path"] +"\" -n "+ entityDialog.SelectedItem+ " -x \"" + xmiPath + "\" -s " + config["namespace"];

                    process.Start();
                    process.WaitForExit();


                    

                    break;
                case menuSettings:
                    var settingsDialog = new SettingsDialogs(configPath);
                    settingsDialog.ShowDialog();

                    break;
            }
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