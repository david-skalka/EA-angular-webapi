using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EADotnetWebapiCli
{




    [Verb("initialize-api", HelpText = "Initialize api project")]
    class InitializeApiOptions
    {
        [Option('o', "output-dir", Required = true)]
        public string OutputDir { get; set; } = String.Empty;

        [Option('n', "project-name", Required = true)]
        public string ProjectName { get; set; } = String.Empty;
    }


    [Verb("initialize-client", HelpText = "Initialize client project")]
    class InitializeClientOptions
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



    [Verb("global-mock-data", HelpText = "Global mock data")]
    class GlobalMockDataOptions
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



    [Verb("app-component", HelpText = "App component")]
    class AppComponentOptions
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



    [Verb("app-routes", HelpText = "Routes")]
    class AppRoutesOptions
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





    [Verb("app-routes", HelpText = "Routes")]
    class AppComponentsOptions
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
