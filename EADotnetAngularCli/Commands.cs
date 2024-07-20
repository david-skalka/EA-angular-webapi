using Bogus.DataSets;
using Sharprompt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EADotnetAngularCli
{

    public interface IGeneratorCommand
    {
        void Execute();
    }


    public class WriteCallbackResultGeneratorCommand : IGeneratorCommand
    {
        public Func<string> callback;

        public string path;

        private readonly bool force;

        public WriteCallbackResultGeneratorCommand(Func<string> callback, string path, bool force)
        {
            this.callback = callback;
            this.path = path;
            this.force = force;
        }

       bool canWrite()
        {
            if (!File.Exists(path))
            {
                return true;
            }

            if (force)
            {
                return true;
            }

            var confirm = Prompt.Confirm("File " + path + " already exists. Do you want to overwrite it? (y/n)", defaultValue: false);

            
            if (confirm)
            {
                return true;
            }


            return false;
        }


        public void Execute()
        {
            if(canWrite())
            {
                File.WriteAllText(path, callback());
            }
        }
    }


    public class ShellGeneratorCommand : IGeneratorCommand
    {
        private string filename;
        private string args;
        private readonly string? cwd;

        public ShellGeneratorCommand(string filename, string args, string? cwd)
        {
            this.filename = filename;
            this.args = args;
            this.cwd = cwd;
        }

        public void Execute()
        {
            var process = new System.Diagnostics.Process();
            process.StartInfo.FileName = filename;
            process.StartInfo.Arguments = args;
            process.StartInfo.WorkingDirectory = cwd;
            process.StartInfo.UseShellExecute=true;
            process.Start();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                
                throw new Exception("Error executing command " + filename + " " + args);
            }
        }
    }


    public class RmGeneratorCommand : IGeneratorCommand
    {
        private string searchPattern;

        private string directoryPath;

        public RmGeneratorCommand(string directoryPath, string searchPattern)
        {
            this.directoryPath = directoryPath;
            this.searchPattern = searchPattern;
            
        }

        public void Execute()
        {
            var files = Directory.GetFiles(directoryPath, searchPattern);

            foreach (var item in files)
            {
                File.Delete(item);
            }
        }
    }



    public class RmDirGeneratorCommand : IGeneratorCommand
    {
  

        private string directoryPath;

        public RmDirGeneratorCommand(string directoryPath)
        {
            this.directoryPath = directoryPath;
  

        }

        public void Execute()
        {
            Directory.Delete(directoryPath, true);
        }
    }


    public class MkdirGeneratorCommand : IGeneratorCommand
    {
        private string path;

        public MkdirGeneratorCommand(string path)
        {
            this.path = path;
        }

        public void Execute()
        {
            Directory.CreateDirectory(path);
        }
    }




    public class JsonCommand : IGeneratorCommand
    {
        private string path;

        private Func<dynamic, dynamic> func;

        public JsonCommand(string path, Func<dynamic, dynamic> func)
        {
            this.path = path;
            this.func = func;
        }

        public void Execute()
        {
            var des = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(path))!;

            var output = func(des);

            File.WriteAllText(path, Newtonsoft.Json.JsonConvert.SerializeObject(output, Newtonsoft.Json.Formatting.Indented));

            
        }
    }
}
