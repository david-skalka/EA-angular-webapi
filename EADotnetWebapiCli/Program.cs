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
            Environment.Exit(1);
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
    ExecShell("dotnet", "new webapi -f net8.0 -n " + options.Namespace + " -o " + options.Path, null);
    Directory.CreateDirectory(Path.Combine(options.Path, "Models"));
    Directory.CreateDirectory(Path.Combine(options.Path, "Controllers"));
    ExecShell("dotnet", "add package Microsoft.EntityFrameworkCore --version 6.0.27", options.Path);
    ExecShell("dotnet", "add package Microsoft.EntityFrameworkCore.Sqlite --version 6.0.27", options.Path);


    var programContent = new EADotnetWebapiCli.Templates.Program().TransformText();
    File.WriteAllText(Path.Combine(options.Path, "Program.cs"), programContent);
    return 0;
},
(DbContextOptions options) =>
{

    var dbContext = new DbContext();
    dbContext.Namespace = options.Namespace;    
    dbContext.Entities = new EAXmiParser().Parse(options.Xmi).Where(x=>x.Stereotype=="DotnetWebapi:Entity").ToArray();
    var programContent = dbContext.TransformText();
    ConfirmFileWrite(Path.Combine(options.Path, "ApplicationDbContext.cs"), programContent);
    return 0;
},
(EntityOptions options) =>
{
    var parser = new EAXmiParser();
    var diagram = parser.Parse(options.Xmi);
    
    var efModel = new EfModel();
    efModel.Model = diagram.Single(x => x.Name == options.Name);
    efModel.Namespace = options.Namespace;
    var efModelContent = efModel.TransformText();

    ConfirmFileWrite(Path.Combine(options.Path, "Models", options.Name + ".cs"), efModelContent);

    var controller = new Controller();
    controller.Model = diagram.Single(x => x.Name == options.Name);
    controller.Namespace = options.Namespace;
    var controllerContent = controller.TransformText();

    ConfirmFileWrite(Path.Combine(options.Path, "Controllers", options.Name + "Controller.cs"), controllerContent);




    return 0;
}
, errors => 1);



[Verb("initialize", HelpText = "Initialize project")]
class InitializeOptions
{
    [Option('p', "path", Required = true, HelpText = "Path to the project")]
    public string Path { get; set; } = String.Empty;

    [Option('s', "namespace", Required = true, HelpText = "Namespace")]
    public string Namespace { get; set; } = String.Empty;
}



[Verb("entity", HelpText = "Generate Entity")]
class EntityOptions
{
    [Option('p', "path", Required = true, HelpText = "Path to the project")]
    public string Path { get; set; } = String.Empty;

    [Option('n', "name", Required = true, HelpText = "Entity name")]
    public string Name { get; set; } = String.Empty;

    [Option('x', "xmi", Required = true, HelpText = "Path to the xmi file")]
    public string Xmi { get; set; } = String.Empty;

    [Option('s', "namespace", Required = true, HelpText = "Namespace")]
    public string Namespace { get; set; } = String.Empty;
}


[Verb("db-context", HelpText = "DbContext")]
class DbContextOptions
{
    [Option('p', "path", Required = true, HelpText = "Path to the project")]
    public string Path { get; set; } = String.Empty;

    [Option('x', "xmi", Required = true, HelpText = "Path to the xmi file")]
    public string Xmi { get; set; } = String.Empty;

    [Option('s', "namespace", Required = true, HelpText = "Namespace")]
    public string Namespace { get; set; } = String.Empty;
}


