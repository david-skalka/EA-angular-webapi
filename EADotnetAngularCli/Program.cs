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


public partial class Program
{


    static IEnumerable<string> GetDependencies(IEnumerable<Element> diagram, string name)
    {
        var retD = diagram.Single(x => x.Name == name).Attributes.Where(x => !x.Type.IsPrimitive).Select(x => x.Type.Name);

        return retD;
    }





    static int Main(string[] args)
    {

        return Parser.Default.ParseArguments<RunPipeline>(args)
                    .MapResult(options =>
                    {


                        var projectName = options.ProjectName;

                        var outputDir = Path.Combine(Directory.GetCurrentDirectory(), options.OutputDir);

                        var testProjectPath = Path.Combine(outputDir, projectName + "IntegrationTest");

                        var overwrite = options.Overwrite;


                        var clientProjectPath = Path.Combine(outputDir, projectName + "Client");


                        Element[] parsedXmi = new EAXmiParser().Parse(options.Xmi);

                        Element[] elements = parsedXmi.Select(x => x.Name).OrderTopologicallyBy(name => GetDependencies(parsedXmi, name)).Select(x => parsedXmi.Single(y => y.Name == x)).ToArray();


                        Element[] entities = elements.Where(x => x.Stereotype == "DotnetAngular:Entity").ToArray();




                        var pipeline = new Dictionary<string, IGeneratorCommand[]> {
                          { "initialize-solution",  new IGeneratorCommand[]{
                            new ShellGeneratorCommand("dotnet", "new sln -n " + projectName + " -o " + outputDir + '"', null) ,
                            new ShellGeneratorCommand("dotnet", "new webapi -f net8.0 -n " + projectName + " -o " + Path.Combine(outputDir, projectName), null),
                            new MkdirGeneratorCommand(Path.Combine(outputDir, projectName, "Models")),
                            new MkdirGeneratorCommand(Path.Combine(outputDir, projectName, "Controllers")),
                            new ShellGeneratorCommand("dotnet", "add package Microsoft.EntityFrameworkCore --version 8.0.6", Path.Combine(outputDir, projectName)),
                            new ShellGeneratorCommand("dotnet", "add package Microsoft.EntityFrameworkCore.Sqlite --version 8.0.6", Path.Combine(outputDir, projectName)),
                            new WriteCallbackResultGeneratorCommand(() => new EADotnetAngularCli.Templates.Api.Program() { ProjectName = projectName }.TransformText(), Path.Combine(outputDir, projectName, "Program.cs"), true),
                            new ShellGeneratorCommand("dotnet", "new nunit -f net8.0 -n " + projectName + "IntegrationTest -o \"" + testProjectPath, null),
                            new MkdirGeneratorCommand(Path.Combine(testProjectPath, "Seeders")),
                            new ShellGeneratorCommand("dotnet", "add package Microsoft.AspNetCore.Mvc.Testing --version 8.0.6", testProjectPath),
                            new ShellGeneratorCommand("dotnet", "add reference ../" + projectName, testProjectPath),
                            new WriteCallbackResultGeneratorCommand(() => new ISeeder() { ProjectName = projectName }.TransformText(), Path.Combine(testProjectPath, "ISeeder.cs"), true),
                            new WriteCallbackResultGeneratorCommand(() => new CustomWebApplicationFactory() { ProjectName = projectName }.TransformText(), Path.Combine(testProjectPath, "CustomWebApplicationFactory.cs"), true),
                            new ShellGeneratorCommand("dotnet", "dotnet sln " + projectName + ".sln add " + projectName + " " + testProjectPath, outputDir)}
                          } ,


                          { "initialize-angular",  new IGeneratorCommand[]{

                            new ShellGeneratorCommand("npx", "@angular/cli@18.0.7 new " + projectName + "Client --style scss --ssr false", outputDir),
                            new JsonCommand(Path.Combine(clientProjectPath, "angular.json"), (dynamic des) => {
                                ((JObject)des).Add("cli", JToken.FromObject(new { analytics = false }));
                                return des;
                            }),
                            new JsonCommand(Path.Combine(clientProjectPath, "angular.json"), (dynamic des) => {
                                ((JObject)des.projects[projectName + "Client"].architect.serve).Add("options", JToken.FromObject(new { proxyConfig = "proxy.conf.json" }));
                                return des;
                            }),
                            new WriteCallbackResultGeneratorCommand(() => new ProxyConf() { }.TransformText(), Path.Combine(clientProjectPath, "proxy.conf.json"), true),
                            new WriteCallbackResultGeneratorCommand(() => new AppConfig().TransformText(), Path.Combine(outputDir, projectName + "Client", "src", "app", "app.config.ts"), true),
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

                            new WriteCallbackResultGeneratorCommand(() => new EADotnetAngularCli.Templates.Client.Storybook.Main() { }.TransformText(), Path.Combine(clientProjectPath, ".storybook", "main.ts"), true),
                            new WriteCallbackResultGeneratorCommand(() => new EADotnetAngularCli.Templates.Client.Storybook.Preview() { }.TransformText(), Path.Combine(clientProjectPath, ".storybook", "preview.ts"), true),
                            new JsonCommand(Path.Combine(clientProjectPath, "package.json"), (dynamic des) => {
                                    des.scripts["update-api"] = "npx --yes concurrently -k -s first -n \"SB,TEST\" -c \"magenta,blue\"  \"cd..\\" + projectName + "\\ && dotnet run --environment Development --urls https://localhost:7064;http://localhost:5195\" \"npx --yes wait-on http-get://127.0.0.1:5195/swagger/v1/swagger.json && swagger-typescript-api -p http://127.0.0.1:5195/swagger/v1/swagger.json -o ./src -n api.ts\"";
                                    return des;
                                }),
                            new RmGeneratorCommand(Path.Combine(clientProjectPath, "src", "stories"), "*.ts"),
                            new RmGeneratorCommand(Path.Combine(clientProjectPath, "src", "stories"), "*.css"),
                            new RmGeneratorCommand(Path.Combine(clientProjectPath, "src", "stories"), "*.mdx"),

                            new RmDirGeneratorCommand(Path.Combine(clientProjectPath, "src", "stories", "assets")),



                          }},
                          { "db-context", new IGeneratorCommand[]{ new WriteCallbackResultGeneratorCommand(() => new DbContext() { ProjectName = projectName, Entities = entities }.TransformText(), Path.Combine(outputDir, projectName, "ApplicationDbContext.cs"), overwrite) } } ,
                          { "seeder",  new IGeneratorCommand[]{new WriteCallbackResultGeneratorCommand(() => new Seeder() { Entities = entities, ProjectName = projectName, Count = 10 }.TransformText(), Path.Combine(outputDir, projectName + "IntegrationTest", "Seeders", "DefaultSeeder.cs"), overwrite) }  },
                          { "global-mock-data",  new IGeneratorCommand[]{ new WriteCallbackResultGeneratorCommand(() => new EADotnetAngularCli.Templates.Client.Storybook.GlobalMockData() { Entities = entities }.TransformText(), Path.Combine(outputDir, projectName + "Client", ".storybook", "global-mock-data.ts"), overwrite) }  },
                          { "app-component",  new IGeneratorCommand[]{ new WriteCallbackResultGeneratorCommand(() => new AppComponent() { }.TransformText(), Path.Combine(outputDir, projectName + "Client", "src", "app", "app.component.ts"), overwrite), new WriteCallbackResultGeneratorCommand(() => new AppTemplate() { Entities = entities, ProjectName = projectName }.TransformText(), Path.Combine(outputDir, projectName + "Client", "src", "app", "app.component.html"), overwrite) }  },
                          { "app-routes",  new IGeneratorCommand[]{ new WriteCallbackResultGeneratorCommand(() => new AppRoutes() { Entities = entities }.TransformText(), Path.Combine(outputDir, projectName + "Client", "src", "app", "app.routes.ts"), overwrite)}  }

                         };



                        foreach (var entity in entities)
                        {

                            pipeline.Add(string.Format("entity-{0}", entity.Name.ToKebabCase()),
                               new IGeneratorCommand[] {
                            new WriteCallbackResultGeneratorCommand(() => new EfModel() { Model = entity, ProjectName = projectName }.TransformText(), Path.Combine(outputDir, projectName, "Models", entity.Name + ".cs"), overwrite),
                            new WriteCallbackResultGeneratorCommand(() => new Controller() { Model = entity, ProjectName = projectName }.TransformText(), Path.Combine(outputDir, projectName, "Controllers", entity.Name + "Controller.cs"), overwrite),
                            new WriteCallbackResultGeneratorCommand(() => new Test() { Model = entity, ProjectName = projectName }.TransformText(), Path.Combine(outputDir, projectName + "IntegrationTest", entity.Name + "Test.cs"), overwrite),
                            new MkdirGeneratorCommand(Path.Combine(outputDir, projectName + "Client", "src", "app", entity.Name.ToKebabCase() + "-edit")),
                            new WriteCallbackResultGeneratorCommand(() => new EditComponent() { Model = entity }.TransformText(), Path.Combine(outputDir, projectName + "Client", "src", "app", entity.Name.ToKebabCase() + "-edit", entity.Name.ToKebabCase() + "-edit.component.ts"), overwrite),
                            new WriteCallbackResultGeneratorCommand(() => new EditTemplate() { Model = entity }.TransformText(), Path.Combine(outputDir, projectName + "Client", "src", "app", entity.Name.ToKebabCase() + "-edit", entity.Name.ToKebabCase() + "-edit.component.html"), overwrite),
                            new WriteCallbackResultGeneratorCommand(() => "", Path.Combine(outputDir, projectName + "Client", "src", "app", entity.Name.ToKebabCase() + "-edit", entity.Name.ToKebabCase() + "-edit.component.scss"), overwrite),
                            new MkdirGeneratorCommand(Path.Combine(outputDir, projectName + "Client", "src", "app", entity.Name.ToKebabCase() + "-list")),
                            new WriteCallbackResultGeneratorCommand(() => new ListComponent() { Model = entity }.TransformText(), Path.Combine(outputDir, projectName + "Client", "src", "app", entity.Name.ToKebabCase() + "-list", entity.Name.ToKebabCase() + "-list.component.ts"), overwrite),
                            new WriteCallbackResultGeneratorCommand(() => new ListTemplate() { Model = entity }.TransformText(), Path.Combine(outputDir, projectName + "Client", "src", "app", entity.Name.ToKebabCase() + "-list", entity.Name.ToKebabCase() + "-list.component.html"), overwrite),
                            new WriteCallbackResultGeneratorCommand(() => "", Path.Combine(outputDir, projectName + "Client", "src", "app", entity.Name.ToKebabCase() + "-list", entity.Name.ToKebabCase() + "-list.component.scss"), overwrite),
                            new WriteCallbackResultGeneratorCommand(() => new Stories() { Model = entity }.TransformText(), Path.Combine(outputDir, projectName + "Client", "src", "stories", entity.Name.ToKebabCase() + "-list.stories.ts"), overwrite)
                        });

                        }


                        var parts = options.Parts != null ? options.Parts.Split(",").Where(x => x != "") : Prompt.MultiSelect("Select parts", pipeline.Select(x => x.Key));



                        foreach (var part in parts)
                        {
                            pipeline[part].ToList().ForEach(x => x.Execute());
                        }


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


}