// See https://aka.ms/new-console-template for more information

using CommandLine;
using EADotnetWebapiCli;
using EADotnetWebapiCli.Templates;
using Medallion.Collections;
using System.Reflection;
using EADotnetWebapiCli.Templates.Api;
using System.Text.Json.Nodes;
using EADotnetWebapiCli.Templates.Client.Storybook;
using EADotnetWebapiCli.Templates.Client;
using CaseExtensions;
using System.Windows.Input;
using System.IO;
IEnumerable<string> GetDependencies(IEnumerable<Element> diagram, string name)
{
    var retD = diagram.Single(x => x.Name == name).Attributes.Where(x => !x.Type.IsPrimitive).Select(x => x.Type.Name);

    return retD;
}

Parser.Default.ParseArguments<InitializeOptions, DbContextOptions, EntityOptions, SeederOptions, GlobalMockDataOptions>(args).MapResult(
    (InitializeOptions options) =>
{
    var outputDir = Path.Combine(Directory.GetCurrentDirectory(), options.OutputDir);

    var clientProjectPath = Path.Combine(outputDir, options.ProjectName + "Client");

    var testProjectPath = Path.Combine(outputDir, options.ProjectName + "IntegrationTest");

    var pipeline = new IGeneratorCommand[] {

        new ShellGeneratorCommand("dotnet", "new sln -n " + options.ProjectName + " -o " + outputDir + '"', null),
        new ShellGeneratorCommand("dotnet", "new webapi -f net8.0 -n " + options.ProjectName + " -o " + Path.Combine(outputDir, options.ProjectName), null),
        new MkdirGeneratorCommand(Path.Combine(outputDir, options.ProjectName, "Models")),
        new MkdirGeneratorCommand(Path.Combine(outputDir, options.ProjectName, "Controllers")),
        new ShellGeneratorCommand("dotnet", "add package Microsoft.EntityFrameworkCore --version 8.0.6", Path.Combine(outputDir, options.ProjectName)),
        new ShellGeneratorCommand("dotnet", "add package Microsoft.EntityFrameworkCore.Sqlite --version 8.0.6", Path.Combine(outputDir, options.ProjectName)),
        new RmGeneratorCommand(Path.Combine(outputDir, options.ProjectName), "Program.cs"),
        new WriteCallbackResultGeneratorCommand(() => new EADotnetWebapiCli.Templates.Api.Program(){ ProjectName = options.ProjectName }.TransformText(), Path.Combine(outputDir,  options.ProjectName, "Program.cs")),

        new ShellGeneratorCommand("dotnet", "new nunit -f net8.0 -n " + options.ProjectName + "IntegrationTest -o \"" + testProjectPath, null),
        new MkdirGeneratorCommand(Path.Combine(testProjectPath, "Seeders")),
        new ShellGeneratorCommand("dotnet", "add package Microsoft.AspNetCore.Mvc.Testing --version 8.0.6", testProjectPath),
        new ShellGeneratorCommand("dotnet", "add reference ../" + options.ProjectName, testProjectPath),
        new RmGeneratorCommand(Path.Combine(testProjectPath), "UnitTest1.cs"),
        new MkdirGeneratorCommand(Path.Combine(testProjectPath, "Seeders")),
        new WriteCallbackResultGeneratorCommand(() => new ISeeder(){ ProjectName = options.ProjectName }.TransformText(), Path.Combine(testProjectPath, "ISeeder.cs")),
        new WriteCallbackResultGeneratorCommand(() => new CustomWebApplicationFactory(){ ProjectName = options.ProjectName }.TransformText(), Path.Combine(testProjectPath, "CustomWebApplicationFactory.cs")),
        
        new ShellGeneratorCommand("dotnet", "dotnet sln " + options.ProjectName + ".sln add "+options.ProjectName+" " + testProjectPath, outputDir),


        new ShellGeneratorCommand("npx", "@angular/cli@18.0.7 new " + options.ProjectName + "Client --style scss --ssr false", outputDir),
        new ShellGeneratorCommand("npx", "storybook@8.1.11 init --disable-telemetry --yes --no-dev",clientProjectPath),
        new ShellGeneratorCommand("npx", "@angular/cli@18.0.7 add @angular/material --skip-confirmation", clientProjectPath),
        new ShellGeneratorCommand("npx", "i swagger-typescript-api@13.0.12 -D", clientProjectPath),
        new ShellGeneratorCommand("npx", "i storybook-addon-mock@5.0.0 -D", clientProjectPath),
        new JsonCommand(Path.Combine(clientProjectPath, "tsconfig.json"), (dynamic des)=>{
            ((Newtonsoft.Json.Linq.JArray)des.compilerOptions.lib).Add("dom.iterable");
            return des;
        }),
        new RmGeneratorCommand(Path.Combine(clientProjectPath, ".storybook"), "main.ts"),
        new WriteCallbackResultGeneratorCommand(() => new EADotnetWebapiCli.Templates.Client.Storybook.Main(){ }.TransformText(), Path.Combine(clientProjectPath, ".storybook", "main.ts")),
        new RmGeneratorCommand(Path.Combine(clientProjectPath, ".storybook"), "preview.ts"),
        new WriteCallbackResultGeneratorCommand(() => new EADotnetWebapiCli.Templates.Client.Storybook.Preview(){ }.TransformText(), Path.Combine(clientProjectPath, ".storybook", "preview.ts")),
        new JsonCommand(Path.Combine(clientProjectPath, "package.json"), (dynamic des)=>{
            des.scripts["update-api"]= "swagger-typescript-api -p http://localhost:5291/swagger/v1/swagger.json -o ./src -n api.ts";
            return des;
        }),

        new RmGeneratorCommand(Path.Combine(clientProjectPath, "src", "stories"), "*.ts"),
        new RmGeneratorCommand(Path.Combine(clientProjectPath, "src", "stories"), "*.css"),
        new RmGeneratorCommand(Path.Combine(clientProjectPath, "src", "stories"), "*.mdx"),
        new RmDirGeneratorCommand(Path.Combine(clientProjectPath, "src", "stories", "assets")),
    };
    Generate(pipeline);
    return 0;
},
(DbContextOptions options) =>
{
    var outputDir = Path.Combine(Directory.GetCurrentDirectory(), options.OutputDir);
    var pipeline = new[] {
        new WriteCallbackResultGeneratorCommand(() => new DbContext(){ProjectName = options.ProjectName, Entities = new EAXmiParser().Parse(options.Xmi).Where(x => x.Stereotype == "DotnetWebapi:Entity").Where(x => options.Entities.Split(",").Contains(x.Name)).ToArray() }.TransformText(), Path.Combine(outputDir, options.ProjectName, "ApplicationDbContext.cs"))
    };
    Generate(pipeline);
    return 0;
},
(EntityOptions options) =>
{

    var outputDir = Path.Combine(Directory.GetCurrentDirectory(), options.OutputDir);


    var parser = new EAXmiParser();
    var diagram = parser.Parse(options.Xmi);

    foreach (var entity in options.Entities.Split(","))
    {
        var pipeline = new IGeneratorCommand[] {
            new WriteCallbackResultGeneratorCommand(() => new EfModel(){ Model = diagram.Single(x => x.Name == entity), ProjectName = options.ProjectName }.TransformText(), Path.Combine(outputDir, options.ProjectName, "Models", entity + ".cs")),
            new WriteCallbackResultGeneratorCommand(() =>new Controller(){ Model = diagram.Single(x => x.Name == entity), ProjectName = options.ProjectName}.TransformText()  , Path.Combine(outputDir, options.ProjectName,  "Controllers", entity + "Controller.cs")),
            new WriteCallbackResultGeneratorCommand(() => new Test(){Model = diagram.Single(x => x.Name == entity), ProjectName = options.ProjectName }.TransformText(), Path.Combine(outputDir, options.ProjectName+ "IntegrationTest", entity + "Test.cs")),

            new MkdirGeneratorCommand(Path.Combine(outputDir, options.ProjectName+ "Client", "src", "app", entity.ToKebabCase() + "-edit")),
            new WriteCallbackResultGeneratorCommand(() => new EditComponent(){Model = diagram.Single(x => x.Name == entity)}.TransformText(), Path.Combine(outputDir, options.ProjectName+ "Client", "src", "app", entity.ToKebabCase() + "-edit", entity.ToKebabCase() + "-edit.component.ts")),
            new WriteCallbackResultGeneratorCommand(() => new EditTemplate(){Model = diagram.Single(x => x.Name == entity)}.TransformText(), Path.Combine(outputDir, options.ProjectName+ "Client", "src", "app", entity.ToKebabCase() + "-edit", entity.ToKebabCase() + "-edit.component.html")),
            new WriteCallbackResultGeneratorCommand(() => "", Path.Combine(outputDir, options.ProjectName+ "Client", "src", "app", entity.ToKebabCase() + "-edit" , entity.ToKebabCase() + "-edit.component.scss")),

            new MkdirGeneratorCommand(Path.Combine(outputDir, options.ProjectName+ "Client", "src", "app", entity.ToKebabCase() + "-list")),
            new WriteCallbackResultGeneratorCommand(() => new ListComponent(){Model = diagram.Single(x => x.Name == entity)}.TransformText(), Path.Combine(outputDir, options.ProjectName+ "Client", "src", "app", entity.ToKebabCase() + "-list", entity.ToKebabCase() + "-list.component.ts")),
            new WriteCallbackResultGeneratorCommand(() => new ListTemplate(){Model = diagram.Single(x => x.Name == entity)}.TransformText(), Path.Combine(outputDir, options.ProjectName+ "Client", "src", "app", entity.ToKebabCase() + "-list", entity.ToKebabCase() + "-list.component.html")),
            new WriteCallbackResultGeneratorCommand(() => "", Path.Combine(outputDir, options.ProjectName+ "Client", "src", "app", entity.ToKebabCase() + "-list" , entity.ToKebabCase() + "-list.component.scss")),
            new WriteCallbackResultGeneratorCommand(() => new Stories(){Model = diagram.Single(x => x.Name == entity)}.TransformText(), Path.Combine(outputDir, options.ProjectName+ "Client", "src", "stories", entity.ToKebabCase() + "-list.stories.ts")),

        };
        Generate(pipeline);
    }

    return 0;
},
(SeederOptions options) =>
{

    var diagram = new EAXmiParser().Parse(options.Xmi).Where(x => x.Stereotype == "DotnetWebapi:Entity").Where(x => options.Entities.Split(",").Contains(x.Name)).ToArray();

    var sortedTypes = diagram.Select(x => x.Name).OrderTopologicallyBy(name => GetDependencies(diagram, name)).ToList();

    var diagramSorted = sortedTypes.Select(x => diagram.Single(y => y.Name == x)).ToArray();

    var outputDir = Path.Combine(Directory.GetCurrentDirectory(), options.OutputDir);

    var pipeline = new[] {
        new WriteCallbackResultGeneratorCommand(() => new Seeder(){Entities = diagramSorted, ProjectName = options.ProjectName,  Count = 10 }.TransformText(), Path.Combine(outputDir, options.ProjectName + "IntegrationTest",  "Seeders", "DefaultSeeder.cs"))
    };
    Generate(pipeline);
    return 0;
},
(GlobalMockDataOptions options) =>
{

    var diagram = new EAXmiParser().Parse(options.Xmi).Where(x => x.Stereotype == "DotnetWebapi:Entity").Where(x => options.Entities.Split(",").Contains(x.Name)).ToArray();

    var sortedTypes = diagram.Select(x => x.Name).OrderTopologicallyBy(name => GetDependencies(diagram, name)).ToList();

    var diagramSorted = sortedTypes.Select(x => diagram.Single(y => y.Name == x)).ToArray();

    var outputDir = Path.Combine(Directory.GetCurrentDirectory(), options.OutputDir);



    var pipeline = new[] {
        new WriteCallbackResultGeneratorCommand(() => new GlobalMockData(){}.TransformText(), Path.Combine(outputDir, options.ProjectName + "Client", ".storybook", "global-mock-data.ts"))
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

