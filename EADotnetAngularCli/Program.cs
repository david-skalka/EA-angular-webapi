// See https://aka.ms/new-console-template for more information

using CommandLine;
using EADotnetAngularCli;
using Medallion.Collections;
using EADotnetAngularCli.Templates.Api;
using EADotnetAngularCli.Templates.Client;
using CaseExtensions;
using Newtonsoft.Json.Linq;

IEnumerable<string> GetDependencies(IEnumerable<Element> diagram, string name)
{
    var retD = diagram.Single(x => x.Name == name).Attributes.Where(x => !x.Type.IsPrimitive).Select(x => x.Type.Name);

    return retD;
}


Parser.Default.ParseArguments<Options>(args)
                  .WithParsed(options =>
                  {

                      var parts = new Dictionary<string, Func<PipelineOptions, IGeneratorCommand[]>> {
                          { "initialize-solution", (PipelineOptions options)=>{


                               var outputDir = options.OutputDir;

                                var testProjectPath = Path.Combine(outputDir, options.ProjectName + "IntegrationTest");

                                return new IGeneratorCommand[] {

                                    new ShellGeneratorCommand("dotnet", "new sln -n " + options.ProjectName + " -o " + outputDir + '"', null),
                                    new ShellGeneratorCommand("dotnet", "new webapi -f net8.0 -n " + options.ProjectName + " -o " + Path.Combine(outputDir, options.ProjectName), null),
                                    new MkdirGeneratorCommand(Path.Combine(outputDir, options.ProjectName, "Models")),
                                    new MkdirGeneratorCommand(Path.Combine(outputDir, options.ProjectName, "Controllers")),
                                    new ShellGeneratorCommand("dotnet", "add package Microsoft.EntityFrameworkCore --version 8.0.6", Path.Combine(outputDir, options.ProjectName)),
                                    new ShellGeneratorCommand("dotnet", "add package Microsoft.EntityFrameworkCore.Sqlite --version 8.0.6", Path.Combine(outputDir, options.ProjectName)),
                                    new WriteCallbackResultGeneratorCommand(() => new EADotnetAngularCli.Templates.Api.Program(){ ProjectName = options.ProjectName }.TransformText(), Path.Combine(outputDir,  options.ProjectName, "Program.cs"), true),
                                    new ShellGeneratorCommand("dotnet", "new nunit -f net8.0 -n " + options.ProjectName + "IntegrationTest -o \"" + testProjectPath, null),
                                    new MkdirGeneratorCommand(Path.Combine(testProjectPath, "Seeders")),
                                    new ShellGeneratorCommand("dotnet", "add package Microsoft.AspNetCore.Mvc.Testing --version 8.0.6", testProjectPath),
                                    new ShellGeneratorCommand("dotnet", "add reference ../" + options.ProjectName, testProjectPath),
                                    new MkdirGeneratorCommand(Path.Combine(testProjectPath, "Seeders")),
                                    new WriteCallbackResultGeneratorCommand(() => new ISeeder(){ ProjectName = options.ProjectName }.TransformText(), Path.Combine(testProjectPath, "ISeeder.cs"), true),
                                    new WriteCallbackResultGeneratorCommand(() => new CustomWebApplicationFactory(){ ProjectName = options.ProjectName }.TransformText(), Path.Combine(testProjectPath, "CustomWebApplicationFactory.cs"), true),

                                    new ShellGeneratorCommand("dotnet", "dotnet sln " + options.ProjectName + ".sln add "+options.ProjectName+" " + testProjectPath, outputDir)

                                };



                          } },

                          

                          { "initialize-angular", (PipelineOptions options)=>{


                                var outputDir = options.OutputDir;

                                var clientProjectPath = Path.Combine(outputDir, options.ProjectName + "Client");

                                var testProjectPath = Path.Combine(outputDir, options.ProjectName + "IntegrationTest");

                                return new IGeneratorCommand[] {

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
                                    new ShellGeneratorCommand("npx", "--yes playwright install --with-deps", clientProjectPath),


                                    new JsonCommand(Path.Combine(clientProjectPath, "package.json"), (dynamic des)=>{
                                        des.scripts["start"]= "ng serve --ssl";
                                        ((JObject)des.scripts).Add("test-storybook", "test-storybook");
                                        return des;
                                    }),

                                    new JsonCommand(Path.Combine(clientProjectPath, "tsconfig.json"), (dynamic des)=>{
                                        ((JArray)des.compilerOptions.lib).Add("dom.iterable");
                                        return des;
                                    }),

                                    new WriteCallbackResultGeneratorCommand(() => new EADotnetAngularCli.Templates.Client.Storybook.Main(){ }.TransformText(), Path.Combine(clientProjectPath, ".storybook", "main.ts"), true),

                                    new WriteCallbackResultGeneratorCommand(() => new EADotnetAngularCli.Templates.Client.Storybook.Preview(){ }.TransformText(), Path.Combine(clientProjectPath, ".storybook", "preview.ts"), true),
                                    new JsonCommand(Path.Combine(clientProjectPath, "package.json"), (dynamic des)=>{
                                        des.scripts["update-api"]= "npx --yes concurrently -k -s first -n \"SB,TEST\" -c \"magenta,blue\"  \"cd..\\"+options.ProjectName+"\\ && dotnet run --environment Development --urls https://localhost:7064;http://localhost:5195\" \"npx --yes wait-on http-get://127.0.0.1:5195/swagger/v1/swagger.json && swagger-typescript-api -p http://127.0.0.1:5195/swagger/v1/swagger.json -o ./src -n api.ts\"";
                                        return des;
                                    }),
                                    new ShellGeneratorCommand("npm", "i cross-env -D", clientProjectPath),



                                    new RmGeneratorCommand(Path.Combine(clientProjectPath, "src", "stories"), "*.ts"),
                                        new RmGeneratorCommand(Path.Combine(clientProjectPath, "src", "stories"), "*.css"),
                                        new RmGeneratorCommand(Path.Combine(clientProjectPath, "src", "stories"), "*.mdx"),
                                        new RmDirGeneratorCommand(Path.Combine(clientProjectPath, "src", "stories", "assets")),
                                    };

                          } },

                          { "db-context", (PipelineOptions options)=>{

                                    return new[] {
                                        new WriteCallbackResultGeneratorCommand(() => new DbContext(){ProjectName = options.ProjectName, Entities = options.Elements.Where(x => x.Stereotype == "DotnetWebapi:Entity").Where(x => options.Entities.Contains(x.Name)).ToArray() }.TransformText(), Path.Combine(options.OutputDir, options.ProjectName, "ApplicationDbContext.cs"), options.Force)
                                    };

                          } },
                           { "entity", (PipelineOptions options)=>{

                                    var outputDir = options.OutputDir;


                                    
                                    var diagram = options.Elements;

                                    var retD = new List<IGeneratorCommand>();                                

                                    foreach (var entity in options.Entities)
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
                                        retD.AddRange(pipeline);
                                    }
                                    return retD.ToArray();
                          } },
                            { "seeder", (PipelineOptions options)=>{

                                        var diagram = options.Elements.Where(x => x.Stereotype == "DotnetWebapi:Entity").Where(x => options.Entities.Contains(x.Name)).ToArray();

                                        var sortedTypes = diagram.Select(x => x.Name).OrderTopologicallyBy(name => GetDependencies(diagram, name)).ToList();

                                        var diagramSorted = sortedTypes.Select(x => diagram.Single(y => y.Name == x)).ToArray();

                                        

                                        return new[] {
                                            new WriteCallbackResultGeneratorCommand(() => new Seeder(){Entities = diagramSorted, ProjectName = options.ProjectName,  Count = 10 }.TransformText(), Path.Combine(options.OutputDir, options.ProjectName + "IntegrationTest",  "Seeders", "DefaultSeeder.cs"),options.Force)
                                        };

                          } },

                             { "global-mock-data", (PipelineOptions options)=>{

                                        var diagram = options.Elements.Where(x => x.Stereotype == "DotnetWebapi:Entity").Where(x => options.Entities.Contains(x.Name)).ToArray();



                                        return new[] {
                                            new WriteCallbackResultGeneratorCommand(() => new EADotnetAngularCli.Templates.Client.Storybook.GlobalMockData(){ Entities=diagram}.TransformText(), Path.Combine(options.OutputDir, options.ProjectName + "Client", ".storybook", "global-mock-data.ts"),options.Force)
                                        };

                          } },
                             { "app-component", (PipelineOptions options) =>
                             {

                                        var diagram = options.Elements.Where(x => x.Stereotype == "DotnetWebapi:Entity").Where(x => options.Entities.Contains(x.Name)).ToArray();

                                        

                                        return new[] {
                                            new WriteCallbackResultGeneratorCommand(() => new AppComponent(){ }.TransformText(), Path.Combine(options.OutputDir, options.ProjectName + "Client", "src", "app", "app.component.ts"),options.Force),
                                            new WriteCallbackResultGeneratorCommand(() => new AppTemplate(){Entities=diagram , ProjectName=options.ProjectName }.TransformText(), Path.Combine(options.OutputDir, options.ProjectName + "Client", "src", "app", "app.component.html"),options.Force)
                                        };

                            } },


                            { "app-routes", (PipelineOptions options) =>
                             {
                                    var diagram = options.Elements.Where(x => x.Stereotype == "DotnetWebapi:Entity").Where(x => options.Entities.Contains(x.Name)).ToArray();

                                    

                                    return new[] {
                                        new WriteCallbackResultGeneratorCommand(() => new AppRoutes(){ Entities=diagram }.TransformText(), Path.Combine(options.OutputDir, options.ProjectName + "Client", "src", "app", "app.routes.ts"),options.Force),
                                    };

                            } },


                      };


                      var pipelineOptions = new PipelineOptions(options.Parts.Split(","), Path.Combine(Directory.GetCurrentDirectory(), options.OutputDir), options.Entities.Split(","), new EAXmiParser().Parse(options.Xmi), options.ProjectName, options.Force);

                      foreach (var part in options.Parts.Split(","))
                      {
                          parts[part](pipelineOptions).ToList().ForEach(x => x.Execute());
                      }

                  });






class Options
{
    [Option('p', "parts", Required = true)]
    public string Parts { get; set; } = String.Empty;

    [Option('o', "output-dir", Required = true)]
    public string OutputDir { get; set; } = String.Empty;

    [Option('e', "entities", Required = true)]
    public string Entities { get; set; } = String.Empty;

    [Option('x', "xmi", Required = true)]
    public string Xmi { get; set; } = String.Empty;

    [Option('n', "project-name", Required = true)]
    public string ProjectName { get; set; } = String.Empty;

    [Option('f', "force", Default = false)]
    public bool Force { get; set; } = false;

}



class PipelineOptions
{
    
    public string[] Parts { get; set; }

    public string OutputDir { get; set; }

    public string[] Entities { get; set; }

    public Element[] Elements { get; set; }

    public string ProjectName { get; set; }

    public bool Force { get; set; }

    public PipelineOptions(string[] parts, string outputDir, string[] entities, Element[] elements, string projectName, bool force)
    {
        Parts = parts;
        OutputDir = outputDir;
        Entities = entities;
        Elements = elements;
        ProjectName = projectName;
        Force = force;
    }

}


