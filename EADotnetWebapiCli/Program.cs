// See https://aka.ms/new-console-template for more information

using CommandLine;
using EADotnetWebapiCli;
using Medallion.Collections;
using EADotnetWebapiCli.Templates.Api;
using EADotnetWebapiCli.Templates.Client;
using CaseExtensions;
using Newtonsoft.Json.Linq;
using System.Text;
IEnumerable<string> GetDependencies(IEnumerable<Element> diagram, string name)
{
    var retD = diagram.Single(x => x.Name == name).Attributes.Where(x => !x.Type.IsPrimitive).Select(x => x.Type.Name);

    return retD;
}




Parser.Default.ParseArguments<InitializeApiOptions, InitializeClientOptions, DbContextOptions, EntityOptions, SeederOptions, GlobalMockDataOptions, AppComponentOptions, AppRoutesOptions, AppComponentOptions>(args).MapResult(
    (InitializeApiOptions options) =>
{
    var outputDir = Path.Combine(Directory.GetCurrentDirectory(), options.OutputDir);

    

    var testProjectPath = Path.Combine(outputDir, options.ProjectName + "IntegrationTest");

    var pipeline = new IGeneratorCommand[] {
        
        new ShellGeneratorCommand("dotnet", "new sln -n " + options.ProjectName + " -o " + outputDir + '"', null),
        new ShellGeneratorCommand("dotnet", "new webapi -f net8.0 -n " + options.ProjectName + " -o " + Path.Combine(outputDir, options.ProjectName), null),
        new MkdirGeneratorCommand(Path.Combine(outputDir, options.ProjectName, "Models")),
        new MkdirGeneratorCommand(Path.Combine(outputDir, options.ProjectName, "Controllers")),
        new ShellGeneratorCommand("dotnet", "add package Microsoft.EntityFrameworkCore --version 8.0.6", Path.Combine(outputDir, options.ProjectName)),
        new ShellGeneratorCommand("dotnet", "add package Microsoft.EntityFrameworkCore.Sqlite --version 8.0.6", Path.Combine(outputDir, options.ProjectName)),
        new WriteCallbackResultGeneratorCommand(() => new EADotnetWebapiCli.Templates.Api.Program(){ ProjectName = options.ProjectName }.TransformText(), Path.Combine(outputDir,  options.ProjectName, "Program.cs"), true),
        new ShellGeneratorCommand("dotnet", "new nunit -f net8.0 -n " + options.ProjectName + "IntegrationTest -o \"" + testProjectPath, null),
        new MkdirGeneratorCommand(Path.Combine(testProjectPath, "Seeders")),
        new ShellGeneratorCommand("dotnet", "add package Microsoft.AspNetCore.Mvc.Testing --version 8.0.6", testProjectPath),
        new ShellGeneratorCommand("dotnet", "add reference ../" + options.ProjectName, testProjectPath),
        new MkdirGeneratorCommand(Path.Combine(testProjectPath, "Seeders")),
        new WriteCallbackResultGeneratorCommand(() => new ISeeder(){ ProjectName = options.ProjectName }.TransformText(), Path.Combine(testProjectPath, "ISeeder.cs"), true),
        new WriteCallbackResultGeneratorCommand(() => new CustomWebApplicationFactory(){ ProjectName = options.ProjectName }.TransformText(), Path.Combine(testProjectPath, "CustomWebApplicationFactory.cs"), true),

        new ShellGeneratorCommand("dotnet", "dotnet sln " + options.ProjectName + ".sln add "+options.ProjectName+" " + testProjectPath, outputDir)
        
    };


    Generate(pipeline);
    return 0;
},
    (InitializeClientOptions options) =>
{
    var outputDir = Path.Combine(Directory.GetCurrentDirectory(), options.OutputDir);

    var clientProjectPath = Path.Combine(outputDir, options.ProjectName + "Client");

    var testProjectPath = Path.Combine(outputDir, options.ProjectName + "IntegrationTest");

    var pipeline = new IGeneratorCommand[] {

        new ShellGeneratorCommand("npx", "@angular/cli@18.0.7 new " + options.ProjectName + "Client --style scss --ssr false", outputDir),
        new JsonCommand(Path.Combine(clientProjectPath, "angular.json"), (dynamic des)=>{
            ((JObject)des).Add("cli", JToken.FromObject(new { analytics=false }));
            return des;
        }),
     
     

     

        new JsonCommand(Path.Combine(clientProjectPath, "angular.json"), (dynamic des)=>{
            ((JObject)des.projects[ options.ProjectName + "Client"].architect.serve).Add("options", JToken.FromObject(new { proxyConfig="proxy.conf.json" }));
            return des;
        }),
       
         new RmGeneratorCommand(Path.Combine(clientProjectPath), "proxy.conf.json"),
         new WriteCallbackResultGeneratorCommand(() => new ProxyConf(){ }.TransformText(), Path.Combine(clientProjectPath, "proxy.conf.json"), true),
         new WriteCallbackResultGeneratorCommand(() => new AppConfig().TransformText(), Path.Combine(outputDir, options.ProjectName+ "Client", "src", "app", "app.config.ts"), true),


        new ShellGeneratorCommand("npx", "storybook@8.1.11 init --disable-telemetry --yes --no-dev",clientProjectPath),
        new ShellGeneratorCommand("npx", "@angular/cli@18.0.7 add @angular/material --skip-confirmation --defaults", clientProjectPath),
        new ShellGeneratorCommand("npm", "i swagger-typescript-api@13.0.12 -D", clientProjectPath),
        new ShellGeneratorCommand("npm", "i storybook-addon-mock@5.0.0 -D", clientProjectPath),
        new ShellGeneratorCommand("npm", "i @storybook/test-runner@0.19.1 -D", clientProjectPath),


        new JsonCommand(Path.Combine(clientProjectPath, "package.json"), (dynamic des)=>{
            des.scripts["start"]= "ng serve --ssl";
            ((JObject)des.scripts).Add("test-storybook", "test-storybook");
            return des;
        }),

        new JsonCommand(Path.Combine(clientProjectPath, "tsconfig.json"), (dynamic des)=>{
            ((JArray)des.compilerOptions.lib).Add("dom.iterable");
            return des;
        }),
        
        new WriteCallbackResultGeneratorCommand(() => new EADotnetWebapiCli.Templates.Client.Storybook.Main(){ }.TransformText(), Path.Combine(clientProjectPath, ".storybook", "main.ts"), true),
        
        new WriteCallbackResultGeneratorCommand(() => new EADotnetWebapiCli.Templates.Client.Storybook.Preview(){ }.TransformText(), Path.Combine(clientProjectPath, ".storybook", "preview.ts"), true),
        new JsonCommand(Path.Combine(clientProjectPath, "package.json"), (dynamic des)=>{
            des.scripts["update-api"]= "cross-env ASPNETCORE_ENVIRONMENT=swagger-gen ..\\"+options.ProjectName+"\\bin\\debug\\net8.0\\"+options.ProjectName+".exe > swagger.json  && swagger-typescript-api -p swagger.json -o ./src -n api.ts && del swagger.json";
            return des;
        }),
        new ShellGeneratorCommand("npm", "i cross-env -D", clientProjectPath),

        

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
        new WriteCallbackResultGeneratorCommand(() => new DbContext(){ProjectName = options.ProjectName, Entities = new EAXmiParser().Parse(options.Xmi).Where(x => x.Stereotype == "DotnetWebapi:Entity").Where(x => options.Entities.Split(",").Contains(x.Name)).ToArray() }.TransformText(), Path.Combine(outputDir, options.ProjectName, "ApplicationDbContext.cs"), options.Force)
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
            new WriteCallbackResultGeneratorCommand(() => new EfModel(){ Model = diagram.Single(x => x.Name == entity), ProjectName = options.ProjectName }.TransformText(), Path.Combine(outputDir, options.ProjectName, "Models", entity + ".cs"),options.Force),
            new WriteCallbackResultGeneratorCommand(() =>new Controller(){ Model = diagram.Single(x => x.Name == entity), ProjectName = options.ProjectName}.TransformText()  , Path.Combine(outputDir, options.ProjectName,  "Controllers", entity + "Controller.cs"),options.Force),
            new WriteCallbackResultGeneratorCommand(() => new Test(){Model = diagram.Single(x => x.Name == entity), ProjectName = options.ProjectName }.TransformText(), Path.Combine(outputDir, options.ProjectName+ "IntegrationTest", entity + "Test.cs"),options.Force),

            new MkdirGeneratorCommand(Path.Combine(outputDir, options.ProjectName+ "Client", "src", "app", entity.ToKebabCase() + "-edit")),
            new WriteCallbackResultGeneratorCommand(() => new EditComponent(){Model = diagram.Single(x => x.Name == entity)}.TransformText(), Path.Combine(outputDir, options.ProjectName+ "Client", "src", "app", entity.ToKebabCase() + "-edit", entity.ToKebabCase() + "-edit.component.ts"),options.Force),
            new WriteCallbackResultGeneratorCommand(() => new EditTemplate(){Model = diagram.Single(x => x.Name == entity)}.TransformText(), Path.Combine(outputDir, options.ProjectName+ "Client", "src", "app", entity.ToKebabCase() + "-edit", entity.ToKebabCase() + "-edit.component.html"),options.Force),
            new WriteCallbackResultGeneratorCommand(() => "", Path.Combine(outputDir, options.ProjectName+ "Client", "src", "app", entity.ToKebabCase() + "-edit" , entity.ToKebabCase() + "-edit.component.scss"),options.Force),

            new MkdirGeneratorCommand(Path.Combine(outputDir, options.ProjectName+ "Client", "src", "app", entity.ToKebabCase() + "-list")),
            new WriteCallbackResultGeneratorCommand(() => new ListComponent(){Model = diagram.Single(x => x.Name == entity)}.TransformText(), Path.Combine(outputDir, options.ProjectName+ "Client", "src", "app", entity.ToKebabCase() + "-list", entity.ToKebabCase() + "-list.component.ts"),options.Force),
            new WriteCallbackResultGeneratorCommand(() => new ListTemplate(){Model = diagram.Single(x => x.Name == entity)}.TransformText(), Path.Combine(outputDir, options.ProjectName+ "Client", "src", "app", entity.ToKebabCase() + "-list", entity.ToKebabCase() + "-list.component.html"),options.Force),
            new WriteCallbackResultGeneratorCommand(() => "", Path.Combine(outputDir, options.ProjectName+ "Client", "src", "app", entity.ToKebabCase() + "-list" , entity.ToKebabCase() + "-list.component.scss"),options.Force),
            new WriteCallbackResultGeneratorCommand(() => new Stories(){Model = diagram.Single(x => x.Name == entity)}.TransformText(), Path.Combine(outputDir, options.ProjectName+ "Client", "src", "stories", entity.ToKebabCase() + "-list.stories.ts"),options.Force),



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
        new WriteCallbackResultGeneratorCommand(() => new Seeder(){Entities = diagramSorted, ProjectName = options.ProjectName,  Count = 10 }.TransformText(), Path.Combine(outputDir, options.ProjectName + "IntegrationTest",  "Seeders", "DefaultSeeder.cs"),options.Force)
    };
    Generate(pipeline);
    return 0;
},
(GlobalMockDataOptions options) =>
{

    var diagram = new EAXmiParser().Parse(options.Xmi).Where(x => x.Stereotype == "DotnetWebapi:Entity").Where(x => options.Entities.Split(",").Contains(x.Name)).ToArray();

    var outputDir = Path.Combine(Directory.GetCurrentDirectory(), options.OutputDir);



    var pipeline = new[] {
        new WriteCallbackResultGeneratorCommand(() => new EADotnetWebapiCli.Templates.Client.Storybook.GlobalMockData(){ Entities=diagram}.TransformText(), Path.Combine(outputDir, options.ProjectName + "Client", ".storybook", "global-mock-data.ts"),options.Force)
    };
    Generate(pipeline);
    return 0;
},
(AppComponentOptions options) =>
{

    var diagram = new EAXmiParser().Parse(options.Xmi).Where(x => x.Stereotype == "DotnetWebapi:Entity").Where(x => options.Entities.Split(",").Contains(x.Name)).ToArray();

    var outputDir = Path.Combine(Directory.GetCurrentDirectory(), options.OutputDir);

    var pipeline = new[] {
        new WriteCallbackResultGeneratorCommand(() => new AppComponent(){ }.TransformText(), Path.Combine(outputDir, options.ProjectName + "Client", "src", "app", "app.component.ts"),options.Force),
        new WriteCallbackResultGeneratorCommand(() => new AppTemplate(){Entities=diagram , ProjectName=options.ProjectName }.TransformText(), Path.Combine(outputDir, options.ProjectName + "Client", "src", "app", "app.component.html"),options.Force)
    };
    Generate(pipeline);
    return 0;
},
(AppRoutesOptions options) =>
{

    var diagram = new EAXmiParser().Parse(options.Xmi).Where(x => x.Stereotype == "DotnetWebapi:Entity").Where(x => options.Entities.Split(",").Contains(x.Name)).ToArray();

    var outputDir = Path.Combine(Directory.GetCurrentDirectory(), options.OutputDir);

    var pipeline = new[] {
        new WriteCallbackResultGeneratorCommand(() => new AppRoutes(){ Entities=diagram }.TransformText(), Path.Combine(outputDir, options.ProjectName + "Client", "src", "app", "app.routes.ts"),options.Force),
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



