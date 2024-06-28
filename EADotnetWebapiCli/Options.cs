using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EADotnetWebapiCli
{


    [Verb("initialize", HelpText = "Initialize project")]
    class InitializeOptions
    {
        [Option('o', "output-dir", Required = true)]
        public string OutputDir { get; set; } = String.Empty;

        [Option('n', "project-name", Required = true)]
        public string ProjectName { get; set; } = String.Empty;
    }



    [Verb("entity", HelpText = "Generate Entity")]
    class EntityOptions
    {
        [Option('o', "output-dir", Required = true)]
        public string OutputDir { get; set; } = String.Empty;

        [Option('e', "entities", Required = true)]
        public string Entities { get; set; } = String.Empty;

        [Option('x', "xmi", Required = true)]
        public string Xmi { get; set; } = String.Empty;

        [Option('n', "project-name", Required = true)]
        public string ProjectName { get; set; } = String.Empty;
    }


    [Verb("db-context", HelpText = "DbContext")]
    class DbContextOptions
    {
        [Option('o', "output-dir", Required = true)]
        public string OutputDir { get; set; } = String.Empty;

        [Option('e', "entities", Required = true)]
        public string Entities { get; set; } = String.Empty;

        [Option('x', "xmi", Required = true)]
        public string Xmi { get; set; } = String.Empty;

        [Option('n', "project-name", Required = true)]
        public string ProjectName { get; set; } = String.Empty;
    }





    [Verb("seeder", HelpText = "Seeder")]
    class SeederOptions
    {
        [Option('o', "output-dir", Required = true)]
        public string OutputDir { get; set; } = String.Empty;

        [Option('e', "entities", Required = true)]
        public string Entities { get; set; } = String.Empty;

        [Option('x', "xmi", Required = true)]
        public string Xmi { get; set; } = String.Empty;

        [Option('n', "project-name", Required = true)]
        public string ProjectName { get; set; } = String.Empty;
    }

}
