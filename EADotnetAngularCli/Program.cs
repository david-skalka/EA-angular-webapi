// See https://aka.ms/new-console-template for more information

using CommandLine;
using EADotnetAngularCli;
using Medallion.Collections;
using EADotnetAngularCli.Templates.Api;
using EADotnetAngularCli.Templates.Client;
using CaseExtensions;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Threading.Tasks.Dataflow;
using System.Drawing;
using Sharprompt;
using System.Xml.Linq;


public partial class Program
{


    static IEnumerable<string> GetDependencies(IEnumerable<Element> diagram, string name)
    {
        var retD = diagram.Single(x => x.Name == name).Attributes.Where(x => !x.Type.IsPrimitive).Select(x => x.Type.Name);

        return retD;
    }



    static void Generate(Element[] elements, ICollection<string>? parts, Info info, string outputDir, bool overwrite)
    {

        Element[] elementsSorted = elements.Select(x => x.Name).OrderTopologicallyBy(name => GetDependencies(elements, name)).Select(x => elements.Single(y => y.Name == x)).ToArray();


        Element[] entities = elementsSorted.Where(x => x.Stereotype == "DotnetAngular:Entity").ToArray();

        

        var testProjectPath = Path.Combine(outputDir, info.ProjectName + "IntegrationTest");

        var clientProjectPath = Path.Combine(outputDir, info.ProjectName + "Client");


        var pipeline = new Dictionary<string, IGeneratorCommand[]> {
                          { "initialize-solution",  new IGeneratorCommand[]{
                            new ShellGeneratorCommand("dotnet", "new sln -n " + info.ProjectName + " -o " + outputDir + '"', null) ,
                            new ShellGeneratorCommand("dotnet", "new webapi -f net8.0 -n " + info.ProjectName + " -o " + Path.Combine(outputDir, info.ProjectName), null),
                            new MkdirGeneratorCommand(Path.Combine(outputDir, info.ProjectName, "Models")),
                            new MkdirGeneratorCommand(Path.Combine(outputDir, info.ProjectName, "Controllers")),
                            new ShellGeneratorCommand("dotnet", "add package Microsoft.EntityFrameworkCore --version 8.0.6", Path.Combine(outputDir, info.ProjectName)),
                            new ShellGeneratorCommand("dotnet", "add package Microsoft.EntityFrameworkCore.Sqlite --version 8.0.6", Path.Combine(outputDir, info.ProjectName)),
                            new T4GeneratorCommand(new EADotnetAngularCli.Templates.Api.Program() { Info  = info }, Path.Combine(outputDir, info.ProjectName, "Program.cs"), true),
                            new ShellGeneratorCommand("dotnet", "new nunit -f net8.0 -n " + info.ProjectName + "IntegrationTest -o \"" + testProjectPath, null),
                            new MkdirGeneratorCommand(Path.Combine(testProjectPath, "Seeders")),
                            new ShellGeneratorCommand("dotnet", "add package Microsoft.AspNetCore.Mvc.Testing --version 8.0.6", testProjectPath),
                            new ShellGeneratorCommand("dotnet", "add reference ../" + info.ProjectName, testProjectPath),
                            new T4GeneratorCommand(new ISeeder() { Info=info }, Path.Combine(testProjectPath, "ISeeder.cs"), true),
                            new T4GeneratorCommand(new CustomWebApplicationFactory() { Info=info }, Path.Combine(testProjectPath, "CustomWebApplicationFactory.cs"), true),
                            new ShellGeneratorCommand("dotnet", "dotnet sln " + info.ProjectName + ".sln add " + info.ProjectName + " " + testProjectPath, outputDir)}
                          } ,


                          { "initialize-angular",  new IGeneratorCommand[]{

                            new ShellGeneratorCommand("npx", "@angular/cli@18.0.7 new " + info.ProjectName + "Client --style scss --ssr false", outputDir),
                            new JsonCommand(Path.Combine(clientProjectPath, "angular.json"), (dynamic des) => {
                                ((JObject)des).Add("cli", JToken.FromObject(new { analytics = false }));
                                return des;
                            }),
                            new JsonCommand(Path.Combine(clientProjectPath, "angular.json"), (dynamic des) => {
                                ((JObject)des.projects[info.ProjectName + "Client"].architect.serve).Add("options", JToken.FromObject(new { proxyConfig = "proxy.conf.json" }));
                                return des;
                            }),
                            new T4GeneratorCommand(new ProxyConf() { }, Path.Combine(clientProjectPath, "proxy.conf.json"), true),
                            new T4GeneratorCommand(new AppConfig(), Path.Combine(outputDir, info.ProjectName + "Client", "src", "app", "app.config.ts"), true),
                            new ShellGeneratorCommand("npx", "storybook@8.1.11 init --disable-telemetry --yes --no-dev", clientProjectPath),
                            new ShellGeneratorCommand("npx", "@angular/cli@18.0.7 add @angular/material --skip-confirmation --defaults", clientProjectPath),
                            new ShellGeneratorCommand("npm", "i swagger-typescript-api@13.0.12 -D", clientProjectPath),
                            new ShellGeneratorCommand("npm", "i storybook-addon-mock@5.0.0 -D", clientProjectPath),
                            new ShellGeneratorCommand("npm", "i @storybook/test-runner@0.19.1 -D", clientProjectPath),
                            new ShellGeneratorCommand("npx", "--yes playwright install --with-deps", clientProjectPath),
                            new JsonCommand(Path.Combine(clientProjectPath, "package.json"), (dynamic des) => {
                                    des.scripts["start"] = "ng serve --ssl";
                                    ((JObject)des.scripts).Add("test-storybook", "test-storybook");
                                    return des;
                                }),
                            new JsonCommand(Path.Combine(clientProjectPath, "tsconfig.json"), (dynamic des) => {
                                    ((JArray)des.compilerOptions.lib).Add("dom.iterable");
                                    return des;
                                }),

                            new T4GeneratorCommand(new EADotnetAngularCli.Templates.Client.Storybook.Main() { }, Path.Combine(clientProjectPath, ".storybook", "main.ts"), true),
                            new T4GeneratorCommand(new EADotnetAngularCli.Templates.Client.Storybook.Preview() { }, Path.Combine(clientProjectPath, ".storybook", "preview.ts"), true),
                            new JsonCommand(Path.Combine(clientProjectPath, "package.json"), (dynamic des) => {
                                    des.scripts["update-api"] = "npx --yes concurrently -k -s first -n \"SB,TEST\" -c \"magenta,blue\"  \"cd..\\" + info.ProjectName + "\\ && dotnet run --environment Development --urls https://localhost:7064;http://localhost:5195\" \"npx --yes wait-on http-get://127.0.0.1:5195/swagger/v1/swagger.json && swagger-typescript-api -p http://127.0.0.1:5195/swagger/v1/swagger.json -o ./src -n api.ts\"";
                                    return des;
                                }),
                            new RmGeneratorCommand(Path.Combine(clientProjectPath, "src", "stories"), "*.ts"),
                            new RmGeneratorCommand(Path.Combine(clientProjectPath, "src", "stories"), "*.css"),
                            new RmGeneratorCommand(Path.Combine(clientProjectPath, "src", "stories"), "*.mdx"),

                            new RmDirGeneratorCommand(Path.Combine(clientProjectPath, "src", "stories", "assets")),



                          }},
                          { "db-context", new IGeneratorCommand[]{ new T4GeneratorCommand(new DbContext() { Info=info, Entities = entities }, Path.Combine(outputDir, info.ProjectName, "ApplicationDbContext.cs"), overwrite) } } ,
                          { "seeder",  new IGeneratorCommand[]{new T4GeneratorCommand(new Seeder() { Entities = entities, Info=info}, Path.Combine(outputDir, info.ProjectName + "IntegrationTest", "Seeders", "DefaultSeeder.cs"), overwrite) }  },
                          { "global-mock-data",  new IGeneratorCommand[]{ new T4GeneratorCommand(new EADotnetAngularCli.Templates.Client.Storybook.GlobalMockData() { Entities = entities }, Path.Combine(outputDir, info.ProjectName + "Client", ".storybook", "global-mock-data.ts"), overwrite) }  },
                          { "app-component",  new IGeneratorCommand[]{ new T4GeneratorCommand( new AppComponent() { }, Path.Combine(outputDir, info.ProjectName + "Client", "src", "app", "app.component.ts"), overwrite), new T4GeneratorCommand(new AppTemplate() { Entities = entities, Info = info }, Path.Combine(outputDir, info.ProjectName + "Client", "src", "app", "app.component.html"), overwrite) }  },
                          { "app-routes",  new IGeneratorCommand[]{ new T4GeneratorCommand( new AppRoutes() { Entities = entities }, Path.Combine(outputDir, info.ProjectName + "Client", "src", "app", "app.routes.ts"), overwrite)}  }

                         };



        foreach (var entity in entities)
        {

            pipeline.Add(string.Format("entity-{0}", entity.Name.ToKebabCase()),
               new IGeneratorCommand[] {
                            new T4GeneratorCommand(new EfModel() { Model = entity, Info=info }, Path.Combine(outputDir, info.ProjectName, "Models", entity.Name + ".cs"), overwrite),
                            new T4GeneratorCommand(new Controller() { Model = entity, Info=info }, Path.Combine(outputDir, info.ProjectName, "Controllers", entity.Name + "Controller.cs"), overwrite),
                            new T4GeneratorCommand(new Test() { Model = entity,Info=info }, Path.Combine(outputDir, info.ProjectName + "IntegrationTest", entity.Name + "Test.cs"), overwrite),
                            new MkdirGeneratorCommand(Path.Combine(outputDir, info.ProjectName + "Client", "src", "app", entity.Name.ToKebabCase() + "-edit")),
                            new T4GeneratorCommand(new EditComponent() { Model = entity }, Path.Combine(outputDir, info.ProjectName + "Client", "src", "app", entity.Name.ToKebabCase() + "-edit", entity.Name.ToKebabCase() + "-edit.component.ts"), overwrite),
                            new T4GeneratorCommand(new EditTemplate() { Model = entity }, Path.Combine(outputDir, info.ProjectName + "Client", "src", "app", entity.Name.ToKebabCase() + "-edit", entity.Name.ToKebabCase() + "-edit.component.html"), overwrite),
                            new T4GeneratorCommand(new ListScss(), Path.Combine(outputDir, info.ProjectName + "Client", "src", "app", entity.Name.ToKebabCase() + "-edit", entity.Name.ToKebabCase() + "-edit.component.scss"), overwrite),
                            new MkdirGeneratorCommand(Path.Combine(outputDir, info.ProjectName + "Client", "src", "app", entity.Name.ToKebabCase() + "-list")),
                            new T4GeneratorCommand(new ListComponent() { Model = entity }, Path.Combine(outputDir, info.ProjectName + "Client", "src", "app", entity.Name.ToKebabCase() + "-list", entity.Name.ToKebabCase() + "-list.component.ts"), overwrite),
                            new T4GeneratorCommand(new ListTemplate() { Model = entity }, Path.Combine(outputDir, info.ProjectName + "Client", "src", "app", entity.Name.ToKebabCase() + "-list", entity.Name.ToKebabCase() + "-list.component.html"), overwrite),
                            new T4GeneratorCommand(new ListScss(), Path.Combine(outputDir, info.ProjectName + "Client", "src", "app", entity.Name.ToKebabCase() + "-list", entity.Name.ToKebabCase() + "-list.component.scss"), overwrite),
                            new T4GeneratorCommand(new Stories() { Model = entity }, Path.Combine(outputDir, info.ProjectName + "Client", "src", "stories", entity.Name.ToKebabCase() + "-list.stories.ts"), overwrite)
        });

        }


        var selectedParts = parts != null ? parts : Prompt.MultiSelect("Select parts", pipeline.Select(x => x.Key));



        foreach (var part in selectedParts)
        {
            pipeline[part].ToList().ForEach(x => x.Execute());
        }




    }



    static int Main(string[] args)
    {

        return Parser.Default.ParseArguments<RunPipeline>(args)
                    .MapResult(options =>
                    {

                        var outputDir = Path.Combine(Directory.GetCurrentDirectory(), options.OutputDir);
                        var xmiParser = new EAXmiParser();
                        var parts = options.Parts != null ? options.Parts.Split(",").Where(x => x != null).ToArray() : null;


                        Generate(xmiParser.Parse(options.Xmi), parts, new Info(options.ProjectName, options.SeedCount), outputDir, options.Overwrite);

                       

                        return 0;
                    },error=>1);
    }
    
}







[Verb("run-pipeline")]
class RunPipeline
{
    [Option('d', "output-dir", Required = true)]
    public string OutputDir { get; set; } = String.Empty;

    [Option('x', "xmi", Required = true)]
    public string Xmi { get; set; } = String.Empty;

    [Option('n', "project-name", Required = true)]
    public string ProjectName { get; set; } = String.Empty;

    [Option('o', "overwrite", Default = false)]
    public bool Overwrite { get; set; } = false;

    [Option('p', "parts", Default = null)]
    public  string? Parts { get; set; } = null;


    [Option('s', "seed-count", Default = 10)]
    public int SeedCount { get; set; } = 10;

}