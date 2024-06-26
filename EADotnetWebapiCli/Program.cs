// See https://aka.ms/new-console-template for more information

using CommandLine;
using EADotnetWebapiCli;
using EADotnetWebapiCli.Templates;


void ConfirmFileWrite(string path, string content)
{
    if (File.Exists(path))
    {
        Console.WriteLine("File "+ path + " already exists. Do you want to overwrite it? (y/n)");
        var key = Console.ReadKey();
        if (key.Key != ConsoleKey.Y)
        {
            Console.WriteLine("Aborted");
        }
    }

    File.WriteAllText(path, content);

}


void ExecShell(string filename, string args, string? cwd)
{
    var process = new System.Diagnostics.Process();
    process.StartInfo.FileName = filename;
    process.StartInfo.Arguments = args;
    process.StartInfo.WorkingDirectory = cwd;
    process.Start();
    process.WaitForExit();
}



Parser.Default.ParseArguments<InitializeOptions, DbContextOptions, EntityOptions>(args).MapResult(
    (InitializeOptions options) =>
{
    ExecShell("dotnet", "new webapi -f net8.0 -n " + options.ProjectName + " -o " + options.OutputDir, null);
    Directory.CreateDirectory(Path.Combine(options.OutputDir, "Models"));
    Directory.CreateDirectory(Path.Combine(options.OutputDir, "Controllers"));
    ExecShell("dotnet", "add package Microsoft.EntityFrameworkCore --version 6.0.27", options.OutputDir);
    ExecShell("dotnet", "add package Microsoft.EntityFrameworkCore.Sqlite --version 6.0.27", options.OutputDir);

    var programTemplate = new EADotnetWebapiCli.Templates.Program();
    programTemplate.ProjectName = options.ProjectName;
    var programContent = programTemplate.TransformText();
    File.WriteAllText(Path.Combine(options.OutputDir, "Program.cs"), programContent);
    return 0;
},
(DbContextOptions options) =>
{

    var dbContext = new DbContext();
    dbContext.ProjectName = options.ProjectName;    
    dbContext.Entities = new EAXmiParser().Parse(options.Xmi).Where(x=>x.Stereotype=="DotnetWebapi:Entity").Where(x=> options.Entities.Split(",").Contains(x.Name)).ToArray();
    var programContent = dbContext.TransformText();
    ConfirmFileWrite(Path.Combine(options.OutputDir, "ApplicationDbContext.cs"), programContent);
    return 0;
},
(EntityOptions options) =>
{
    var parser = new EAXmiParser();
    var diagram = parser.Parse(options.Xmi);
    
    var efModel = new EfModel();
    
    
    foreach (var entity in options.Entities.Split(","))
    {
        efModel.Model = diagram.Single(x => x.Name == entity);
        efModel.ProjectName = options.ProjectName;
        var efModelContent = efModel.TransformText();

        ConfirmFileWrite(Path.Combine(options.OutputDir, "Models", options.Entities + ".cs"), efModelContent);

        var controller = new Controller();
        controller.Model = diagram.Single(x => x.Name == entity);
        controller.ProjectName = options.ProjectName;
        var controllerContent = controller.TransformText();

        ConfirmFileWrite(Path.Combine(options.OutputDir, "Controllers", options.Entities + "Controller.cs"), controllerContent);

    }





    return 0;
}
, errors => 1);



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


