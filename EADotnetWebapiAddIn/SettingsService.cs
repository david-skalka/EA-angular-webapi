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

        public string ConfigPath { get; set; }
        

        private Dictionary<string, object> settings;

        public void Load()
        {
            var content = File.ReadAllText(ConfigPath);
            settings = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
        }




        public object GetValue(string key)
        {
            return settings[key];
        }


    }
}
