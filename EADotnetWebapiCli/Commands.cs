using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EADotnetWebapiCli
{

    public interface IGeneratorCommand
    {
        void Execute();
    }


    public class WriteCallbackResultGeneratorCommand : IGeneratorCommand
    {
        public Func<string> callback;

        public string path;


        public WriteCallbackResultGeneratorCommand(Func<string> callback, string path)
        {
            this.callback = callback;
            this.path = path;
        }

        public void Execute()
        {
            if (File.Exists(path))
            {
                Console.WriteLine("File " + path + " already exists. Do you want to overwrite it? (y/n)");
                var key = Console.ReadKey();
                if (key.Key != ConsoleKey.Y)
                {
                    Console.WriteLine("Aborted");
                }
            }
            File.WriteAllText(path, callback());
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
            process.Start();
            process.WaitForExit();
        }
    }


    public class RmGeneratorCommand : IGeneratorCommand
    {
        private string filename;

        public RmGeneratorCommand(string filename)
        {
            this.filename = filename;
        }

        public void Execute()
        {
            File.Delete(filename);
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
}
