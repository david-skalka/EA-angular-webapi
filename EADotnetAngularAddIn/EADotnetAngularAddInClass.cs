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










        public void EA_MenuClick(EA.Repository repository, string Location, string MenuName, string ItemName)
        {

            var commands = new Dictionary<string, Action>
            {

                { menuExecuteGenerator, () =>
                {
                    var settingsDialog = new ExecuteDialog(repository, settingsService);
                    settingsDialog.ShowDialog();

                     


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