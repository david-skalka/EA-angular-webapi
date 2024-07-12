using EADotnetWebapiCli;
using EADotnetWebapiCli.Templates.Client;
using EADotnetWebapiCli.Templates.Client.Storybook;


namespace EADotnetWebapiCliTest
{
    public class ClientTemplatesTest
    {
        private Element[] diagram;

        [SetUp]
        public void Setup()
        {
            var parser = new EAXmiParser();
            diagram = parser.Parse("..\\..\\..\\..\\Data\\Sample\\SampleModel.xml");
        }

        [Test]
        public void EditComponentTest()
        {
            var content = new EditComponent() { Model = diagram.Single(x => x.Name == "Comment"), }.TransformText();
            Console.WriteLine(content);
        }



        [Test]
        public void EditTemplateTest()
        {
            var content = new EditTemplate() { Model = diagram.Single(x => x.Name == "Comment"), }.TransformText();
            Console.WriteLine(content);
        }



        [Test]
        public void ListComponentTest()
        {
            var content = new ListComponent() { Model = diagram.Single(x => x.Name == "Comment"), }.TransformText();
            Console.WriteLine(content);
        }



        [Test]
        public void ListTemplateTest()
        {
            var content = new ListTemplate() { Model = diagram.Single(x => x.Name == "Comment"), }.TransformText();
            Console.WriteLine(content);
        }

        [Test]
        public void StoriesTemplateTest()
        {
            var content = new Stories() { Model = diagram.Single(x => x.Name == "Comment"), }.TransformText();
            Console.WriteLine(content);
        }



        [Test]
        public void ObjectInitializer()
        {
            var model = diagram.Single(x => x.Name == "Product");
            var content = new ObjectInitializer(ElementAutoFaker.GenerateFromElement(model)).ToText();
            Console.WriteLine(content);
        }


        [Test]
        public void GlobalMockData()
        {
            
            var content = new GlobalMockData() { Entities = diagram }.TransformText();
            Console.WriteLine(content);
        }

    }
}