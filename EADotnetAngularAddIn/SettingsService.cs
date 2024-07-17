using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EADotnetWebapiAddIn
{
    public class SettingsService
    {

        
        

        private Dictionary<string, object> settings;

        public SettingsService(string configPath)
        {
            this.configPath = configPath;
        }

        public string configPath;

        public void Load()
        {
            var content = File.ReadAllText(configPath);
            settings = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
        }




        public object GetValue(string key)
        {
            return settings[key];
        }


    }
}
