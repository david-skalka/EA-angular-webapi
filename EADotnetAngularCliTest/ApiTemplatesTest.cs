using EADotnetAngularCli;
using EADotnetAngularCli.Templates.Api;

namespace EADotnetAngularCliTest
{
    public class ApiTemplatesTest
    {
        private Element[] diagram;

        private Info info= new ("Sample", 10);

        [SetUp]
        public void Setup()
        {
            var parser = new EAXmiParser();
            diagram = parser.Parse("..\\..\\..\\..\\Data\\Sample\\SampleModel.xml");
        }

        [Test]
        public void ControllerTest()
        {
            var content = new Controller() { Model = diagram.Single(x => x.Name == "Comment"), Info=info }.TransformText();
            Console.WriteLine(content);
        }



        [Test]
        public void CustomWebApplicationFactory()
        {
            var content = new CustomWebApplicationFactory() { Info = info }.TransformText();
            Console.WriteLine(content);
        }


        [Test]
        public void EfModelTest()
        {
            var content  = new EfModel() { Model = diagram.Single(x => x.Name == "Comment"), Info = info }.TransformText();
            Console.WriteLine(content);
        }

        [Test]
        public void ISeederTest()
        {
            var content = new ISeeder() { Info = info }.TransformText();
            Console.WriteLine(content);
        }

        [Test]
        public void ProgramTest()
        {
            var content = new EADotnetAngularCli.Templates.Api.Program() { Info = info }.TransformText();
            Console.WriteLine(content);
        }

        [Test]
        public void SeederTest()
        {
            var content = new Seeder() { Entities = diagram, Info = info}.TransformText();
            Console.WriteLine(content);
        }


        [Test]
        public void TestTest()
        {
            var content = new Test() { Model = diagram.Single(x => x.Name == "Comment"), Info = info }.TransformText();
            Console.WriteLine(content);
        }




        [Test]
        public void ObjectInitializer()
        {
            var model = diagram.Single(x => x.Name == "Comment");
            var content = new ObjectInitializer(model.Name, ElementAutoFaker.GenerateFromElement(model)) { }.ToText();
            Console.WriteLine(content);
        }
    }
}