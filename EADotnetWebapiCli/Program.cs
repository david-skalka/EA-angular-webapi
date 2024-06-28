// See https://aka.ms/new-console-template for more information

using CommandLine;
using EADotnetWebapiCli;
using EADotnetWebapiCli.Templates;
using Medallion.Collections;
using System.Reflection;

IEnumerable<string> GetDependencies(IEnumerable<Element> diagram,  string name)
{
    var retD = diagram.Single(x=>x.Name==name).Attributes.Where(x => !x.Type.IsPrimitive).Select(x=>x.Type.Name);

    return retD;
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
        new MkdirGeneratorCommand(Path.Combine(options.OutputDir, options.ProjectName+"IntegrationTest", "Seeders")),
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
    var pipeline = new [] {
        new WriteCallbackResultGeneratorCommand(() => new DbContext(){ProjectName = options.ProjectName, Entities = new EAXmiParser().Parse(options.Xmi).Where(x => x.Stereotype == "DotnetWebapi:Entity").Where(x => options.Entities.Split(",").Contains(x.Name)).ToArray() }.TransformText(), Path.Combine(options.OutputDir, options.ProjectName, "ApplicationDbContext.cs"))
    };
    Generate(pipeline);
    return 0;
},
(EntityOptions options) =>
{
    var parser = new EAXmiParser();
    var diagram = parser.Parse(options.Xmi);

    foreach (var entity in options.Entities.Split(","))
    {
        var pipeline = new [] {
            new WriteCallbackResultGeneratorCommand(() => new EfModel(){ Model = diagram.Single(x => x.Name == entity), ProjectName = options.ProjectName }.TransformText(), Path.Combine(options.OutputDir, options.ProjectName, "Models", entity + ".cs")),
            new WriteCallbackResultGeneratorCommand(() =>new Controller(){ Model = diagram.Single(x => x.Name == entity), ProjectName = options.ProjectName}.TransformText()  , Path.Combine(options.OutputDir, options.ProjectName,  "Controllers", entity + "Controller.cs")),
            new WriteCallbackResultGeneratorCommand(() => new Test(){Model = diagram.Single(x => x.Name == entity), ProjectName = options.ProjectName }.TransformText(), Path.Combine(options.OutputDir, options.ProjectName+ "IntegrationTest", entity + "Test.cs"))
        };
        Generate(pipeline);
    }

    return 0;
},
(SeederOptions options) =>
{

    var diagram = new EAXmiParser().Parse(options.Xmi).Where(x => x.Stereotype == "DotnetWebapi:Entity").Where(x => options.Entities.Split(",").Contains(x.Name)).ToArray();

    var sortedTypes = diagram.Select(x=>x.Name).OrderTopologicallyBy(name => GetDependencies(diagram, name)).ToList();

    var diagramSorted = sortedTypes.Select(x => diagram.Single(y => y.Name == x)).ToArray();

    var pipeline = new [] {
        new WriteCallbackResultGeneratorCommand(() => new Seeder(){Entities = diagramSorted, ProjectName = options.ProjectName,  Count = 10 }.TransformText(), Path.Combine(options.OutputDir, options.ProjectName + "IntegrationTest",  "Seeders", "DefaultSeeder.cs"))
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

