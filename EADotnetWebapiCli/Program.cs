// See https://aka.ms/new-console-template for more information

using CommandLine;
using EADotnetWebapiCli;
using EADotnetWebapiCli.Templates;
using System.ComponentModel.Design;
using System.IO;
using System.Windows.Input;





void ExecShell(string filename, string args, string? cwd)
{
    var process = new System.Diagnostics.Process();
    process.StartInfo.FileName = filename;
    process.StartInfo.Arguments = args;
    process.StartInfo.WorkingDirectory = cwd;
    process.Start();
    process.WaitForExit();
}



Parser.Default.ParseArguments<InitializeOptions, DbContextOptions, EntityOptions, SeederOptions>(args).MapResult(
    (InitializeOptions options) =>
{
    var pipeline = new IGeneratorCommand[] {

        new ShellGeneratorCommand("dotnet", "new sln -n " + options.ProjectName + " -o " + options.OutputDir + '"', null),
        new ShellGeneratorCommand("dotnet", "new webapi -f net8.0 -n " + options.ProjectName + " -o " + Path.Combine(options.OutputDir, options.ProjectName), null),
        new MkdirGeneratorCommand(Path.Combine(options.OutputDir, options.ProjectName, "Models")),
        new MkdirGeneratorCommand(Path.Combine(options.OutputDir, options.ProjectName, "Controllers")),
        new ShellGeneratorCommand("dotnet", "add package Microsoft.EntityFrameworkCore --version 6.0.27", Path.Combine(options.OutputDir, options.ProjectName)),
        new ShellGeneratorCommand("dotnet", "add package Microsoft.EntityFrameworkCore.Sqlite --version 6.0.27", Path.Combine(options.OutputDir, options.ProjectName)),
        new ShellGeneratorCommand("dotnet", "new nunit -f net8.0 -n " + options.ProjectName + "IntegrationTest -o \"" + Path.Combine( options.OutputDir, options.ProjectName + "IntegrationTest"), null),
        new ShellGeneratorCommand("dotnet", "add package Microsoft.AspNetCore.Mvc.Testing --version 6.0.27", Path.Combine(options.OutputDir, options.ProjectName+ "IntegrationTest")),
        new ShellGeneratorCommand("dotnet", "add reference ../" + options.ProjectName, Path.Combine(options.OutputDir, options.ProjectName+ "IntegrationTest")),
        new ShellGeneratorCommand("dotnet", "dotnet sln " + options.ProjectName + ".sln add "+options.ProjectName+" "+options.ProjectName+"IntegrationTest", options.OutputDir),
        new RmGeneratorCommand(Path.Combine(options.OutputDir, options.ProjectName, "Program.cs")),
        new WriteCallbackResultGeneratorCommand(() => new EADotnetWebapiCli.Templates.Program(){ ProjectName = options.ProjectName }.TransformText(), Path.Combine(options.OutputDir,  options.ProjectName, "Program.cs")),
        new RmGeneratorCommand(Path.Combine(options.OutputDir, options.ProjectName+"IntegrationTest", "UnitTest1.cs")),
        new MkdirGeneratorCommand(Path.Combine(options.OutputDir, options.ProjectName, options.ProjectName + "IntegrationTest", "Seeders")),
        new WriteCallbackResultGeneratorCommand(() => new ISeeder(){ ProjectName = options.ProjectName }.TransformText(), Path.Combine(options.OutputDir,  options.ProjectName+ "IntegrationTest", "ISeeder.cs")),
        new WriteCallbackResultGeneratorCommand(() => new CustomWebApplicationFactory(){ ProjectName = options.ProjectName }.TransformText(), Path.Combine(options.OutputDir,  options.ProjectName+ "IntegrationTest", "CustomWebApplicationFactory.cs")),
    };
    Generate(pipeline);
    return 0;
},
(DbContextOptions options) =>
{
    var pipeline = new IGeneratorCommand[] {
        new WriteCallbackResultGeneratorCommand(() =>
        {
            var dbContext = new DbContext();
            dbContext.ProjectName = options.ProjectName;
            dbContext.Entities = new EAXmiParser().Parse(options.Xmi).Where(x => x.Stereotype == "DotnetWebapi:Entity").Where(x => options.Entities.Split(",").Contains(x.Name)).ToArray();
            return dbContext.TransformText();

        }, Path.Combine(options.OutputDir, options.ProjectName, "ApplicationDbContext.cs"))
    };
    Generate(pipeline);
    return 0;
},
(EntityOptions options) =>
{
    var parser = new EAXmiParser();
    var diagram = parser.Parse(options.Xmi);


    var fakeObject = new FakerObjectInitializer(diagram.Single(x=>x.Name=="Comment"), new Dictionary<string, object> { { "Id", 1 } });

    var test = fakeObject.ToString();

    foreach (var entity in options.Entities.Split(","))
    {

        var pipeline = new IGeneratorCommand[] {
        new WriteCallbackResultGeneratorCommand(() =>
        {
             var efModel = new EfModel();
           efModel.Model = diagram.Single(x => x.Name == entity);
           efModel.ProjectName = options.ProjectName;
           return efModel.TransformText();

        }, Path.Combine(options.OutputDir, options.ProjectName, "Models", entity + ".cs")),

                new WriteCallbackResultGeneratorCommand(() =>
        {
                 var controller = new Controller();
                controller.Model = diagram.Single(x => x.Name == entity);
                controller.ProjectName = options.ProjectName;
                return controller.TransformText();


  }, Path.Combine(options.OutputDir, options.ProjectName,  "Controllers", entity + "Controller.cs")),



                                new WriteCallbackResultGeneratorCommand(() =>
        {
                 var test = new Test();
                test.Model = diagram.Single(x => x.Name == entity);
                test.ProjectName = options.ProjectName;
                return test.TransformText();


  }, Path.Combine(options.OutputDir, options.ProjectName+ "IntegrationTest", entity + "Test.cs"))




    };
        Generate(pipeline);

    }





    return 0;
},
(SeederOptions options) =>
{
    var parser = new EAXmiParser();
    var diagram = parser.Parse(options.Xmi);

    var pipeline = new IGeneratorCommand[] {


        new WriteCallbackResultGeneratorCommand(() =>
        {
                 var seeder = new Seeder();
                seeder.Entities = diagram;
                seeder.ProjectName = options.ProjectName;
                seeder.Count = 10;
                return seeder.TransformText();


        }, Path.Combine(options.OutputDir, options.ProjectName + "IntegrationTest",  "Seeders", "DefaultSeeder.cs"))
    };
    Generate(pipeline);
    return 0;
}
, errors => 1);


void Generate(IGeneratorCommand[] pipeline)
{
    foreach (var command in pipeline)
    {
        command.Execute();
    }

}

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

    [Option('x', "xmi", Required = true)]
    public string Xmi { get; set; } = String.Empty;

    [Option('n', "project-name", Required = true)]
    public string ProjectName { get; set; } = String.Empty;
}
