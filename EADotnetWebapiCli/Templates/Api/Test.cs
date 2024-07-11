﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 17.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace EADotnetWebapiCli.Templates.Api
{
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Users\pc6vi\source\repos\EA-dotnet-webapi\EADotnetWebapiCli\Templates\Api\Test.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "17.0.0.0")]
    public partial class Test : TestBase
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public virtual string TransformText()
        {
            this.Write("using System.Net.Http.Json;\r\nusing Microsoft.AspNetCore.Mvc.Testing;\r\nusing Micro" +
                    "soft.Extensions.DependencyInjection;\r\nusing ");
            
            #line 9 "C:\Users\pc6vi\source\repos\EA-dotnet-webapi\EADotnetWebapiCli\Templates\Api\Test.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ProjectName));
            
            #line default
            #line hidden
            this.Write(";\r\nusing ");
            
            #line 10 "C:\Users\pc6vi\source\repos\EA-dotnet-webapi\EADotnetWebapiCli\Templates\Api\Test.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ProjectName));
            
            #line default
            #line hidden
            this.Write(".Models;\r\n\r\nnamespace ");
            
            #line 12 "C:\Users\pc6vi\source\repos\EA-dotnet-webapi\EADotnetWebapiCli\Templates\Api\Test.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ProjectName));
            
            #line default
            #line hidden
            this.Write("IntegrationTest\r\n{\r\n    public class ");
            
            #line 14 "C:\Users\pc6vi\source\repos\EA-dotnet-webapi\EADotnetWebapiCli\Templates\Api\Test.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Model.Name));
            
            #line default
            #line hidden
            this.Write("Test\r\n    {\r\n\r\n        protected readonly CustomWebApplicationFactory<Program> Fa" +
                    "ctory = new();\r\n\r\n        protected readonly HttpClient Client;\r\n        \r\n     " +
                    "   public ");
            
            #line 21 "C:\Users\pc6vi\source\repos\EA-dotnet-webapi\EADotnetWebapiCli\Templates\Api\Test.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Model.Name));
            
            #line default
            #line hidden
            this.Write("Test()\r\n        {\r\n            Client = Factory.CreateClient(\r\n                ne" +
                    "w WebApplicationFactoryClientOptions { AllowAutoRedirect = false }\r\n            " +
                    ");\r\n        \r\n        \r\n        }\r\n        \r\n        [SetUp]\r\n        public voi" +
                    "d DbSetup()\r\n        {\r\n            if (TestContext.CurrentContext.Test.Properti" +
                    "es.Get(\"Seeder\") is string seeder)\r\n            {\r\n                ISeeder seede" +
                    "rInstance = (ISeeder)Activator.CreateInstance(Type.GetType(seeder)!)!;\r\n        " +
                    "        seederInstance\r\n                    .Seed(\r\n                        Fact" +
                    "ory\r\n                            .Services.CreateScope()\r\n                      " +
                    "      .ServiceProvider.GetRequiredService<ApplicationDbContext>()\r\n             " +
                    "       );\r\n            }\r\n        \r\n        \r\n        \r\n        \r\n        }\r\n   " +
                    "     \r\n        [TearDown]\r\n        public void DbTeardown()\r\n        {\r\n        " +
                    "    if (TestContext.CurrentContext.Test.Properties.Get(\"Seeder\") is string seede" +
                    "r)\r\n            {\r\n                ISeeder seederInstance = (ISeeder)Activator.C" +
                    "reateInstance(Type.GetType(seeder)!)!;\r\n                seederInstance\r\n        " +
                    "            .Clear(\r\n                            Factory\r\n                      " +
                    "          .Services.CreateScope()\r\n                                .ServiceProvi" +
                    "der.GetRequiredService<ApplicationDbContext>()\r\n                        );\r\n    " +
                    "        }\r\n        }\r\n\r\n\r\n        [OneTimeTearDown]\r\n        public void Dispose" +
                    "()\r\n        {\r\n            Client.Dispose();\r\n            Factory.Dispose();\r\n  " +
                    "      }\r\n\r\n\r\n        [Test, Property(\"Seeder\", \"");
            
            #line 73 "C:\Users\pc6vi\source\repos\EA-dotnet-webapi\EADotnetWebapiCli\Templates\Api\Test.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ProjectName));
            
            #line default
            #line hidden
            this.Write("IntegrationTest.Seeders.DefaultSeeder\")]\r\n        public async Task ");
            
            #line 74 "C:\Users\pc6vi\source\repos\EA-dotnet-webapi\EADotnetWebapiCli\Templates\Api\Test.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Model.Name));
            
            #line default
            #line hidden
            this.Write("GetTest()\r\n        {\r\n            var defaultPage = await Client.GetAsync(\"/");
            
            #line 76 "C:\Users\pc6vi\source\repos\EA-dotnet-webapi\EADotnetWebapiCli\Templates\Api\Test.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Model.Name));
            
            #line default
            #line hidden
            this.Write("\");\r\n            defaultPage.EnsureSuccessStatusCode();\r\n        }\r\n\r\n\r\n\r\n       " +
                    " [Test, Property(\"Seeder\", \"");
            
            #line 82 "C:\Users\pc6vi\source\repos\EA-dotnet-webapi\EADotnetWebapiCli\Templates\Api\Test.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ProjectName));
            
            #line default
            #line hidden
            this.Write("IntegrationTest.Seeders.DefaultSeeder\")]\r\n        public async Task ");
            
            #line 83 "C:\Users\pc6vi\source\repos\EA-dotnet-webapi\EADotnetWebapiCli\Templates\Api\Test.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Model.Name));
            
            #line default
            #line hidden
            this.Write("GetOneTest()\r\n        {\r\n            var defaultPage = await Client.GetAsync(\"/");
            
            #line 85 "C:\Users\pc6vi\source\repos\EA-dotnet-webapi\EADotnetWebapiCli\Templates\Api\Test.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Model.Name));
            
            #line default
            #line hidden
            this.Write("/1\");\r\n            defaultPage.EnsureSuccessStatusCode();\r\n        }\r\n\r\n        [" +
                    "Test, Property(\"Seeder\", \"");
            
            #line 89 "C:\Users\pc6vi\source\repos\EA-dotnet-webapi\EADotnetWebapiCli\Templates\Api\Test.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ProjectName));
            
            #line default
            #line hidden
            this.Write("IntegrationTest.Seeders.DefaultSeeder\")]\r\n        public async Task ");
            
            #line 90 "C:\Users\pc6vi\source\repos\EA-dotnet-webapi\EADotnetWebapiCli\Templates\Api\Test.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Model.Name));
            
            #line default
            #line hidden
            this.Write("CreateTest()\r\n        {\r\n");
            
            #line 92 "C:\Users\pc6vi\source\repos\EA-dotnet-webapi\EADotnetWebapiCli\Templates\Api\Test.tt"
 var createOverride = Model.Attributes.Where(x => !x.Type.IsPrimitive).ToDictionary(x=>x.Name + "Id",x=>(object)1); createOverride["Id"] = null; 
            
            #line default
            #line hidden
            this.Write("            var arg = ");
            
            #line 93 "C:\Users\pc6vi\source\repos\EA-dotnet-webapi\EADotnetWebapiCli\Templates\Api\Test.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(new ObjectInitializer() { Model = Model, Values = ElementAutoFaker.GenerateFromElement(Model, createOverride) }.TransformText()));
            
            #line default
            #line hidden
            this.Write("; \r\n            var defaultPage = await Client.PostAsJsonAsync(\"/");
            
            #line 94 "C:\Users\pc6vi\source\repos\EA-dotnet-webapi\EADotnetWebapiCli\Templates\Api\Test.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Model.Name));
            
            #line default
            #line hidden
            this.Write("\", arg); \r\n            defaultPage.EnsureSuccessStatusCode();\r\n        }\r\n\r\n\r\n   " +
                    "     [Test, Property(\"Seeder\", \"");
            
            #line 99 "C:\Users\pc6vi\source\repos\EA-dotnet-webapi\EADotnetWebapiCli\Templates\Api\Test.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ProjectName));
            
            #line default
            #line hidden
            this.Write("IntegrationTest.Seeders.DefaultSeeder\")]\r\n        public async Task ");
            
            #line 100 "C:\Users\pc6vi\source\repos\EA-dotnet-webapi\EADotnetWebapiCli\Templates\Api\Test.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Model.Name));
            
            #line default
            #line hidden
            this.Write("UpdateTest()\r\n        {\r\n");
            
            #line 102 "C:\Users\pc6vi\source\repos\EA-dotnet-webapi\EADotnetWebapiCli\Templates\Api\Test.tt"
 var updateOverride = Model.Attributes.Where(x => !x.Type.IsPrimitive).ToDictionary(x=>x.Name + "Id", x=>(object)1); updateOverride["Id"] = 1; 
            
            #line default
            #line hidden
            this.Write("            var arg = ");
            
            #line 103 "C:\Users\pc6vi\source\repos\EA-dotnet-webapi\EADotnetWebapiCli\Templates\Api\Test.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(new ObjectInitializer() { Model = Model, Values = ElementAutoFaker.GenerateFromElement(Model, updateOverride) }.TransformText()));
            
            #line default
            #line hidden
            this.Write("; \r\n            var defaultPage = await Client.PutAsJsonAsync(\"/");
            
            #line 104 "C:\Users\pc6vi\source\repos\EA-dotnet-webapi\EADotnetWebapiCli\Templates\Api\Test.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Model.Name));
            
            #line default
            #line hidden
            this.Write("\", arg); \r\n            defaultPage.EnsureSuccessStatusCode();\r\n        }\r\n\r\n\r\n   " +
                    "     \r\n        [Test, Property(\"Seeder\", \"");
            
            #line 110 "C:\Users\pc6vi\source\repos\EA-dotnet-webapi\EADotnetWebapiCli\Templates\Api\Test.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ProjectName));
            
            #line default
            #line hidden
            this.Write("IntegrationTest.Seeders.DefaultSeeder\")]\r\n        public async Task ");
            
            #line 111 "C:\Users\pc6vi\source\repos\EA-dotnet-webapi\EADotnetWebapiCli\Templates\Api\Test.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Model.Name));
            
            #line default
            #line hidden
            this.Write("DeleteTest()\r\n        {\r\n            var defaultPage = await Client.DeleteAsync(\"" +
                    "/");
            
            #line 113 "C:\Users\pc6vi\source\repos\EA-dotnet-webapi\EADotnetWebapiCli\Templates\Api\Test.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Model.Name));
            
            #line default
            #line hidden
            this.Write("/1\"); \r\n            defaultPage.EnsureSuccessStatusCode();\r\n\r\n        }\r\n\r\n\r\n\r\n\r\n" +
                    "    }\r\n}\r\n\r\n\r\n\r\n");
            return this.GenerationEnvironment.ToString();
        }
        
        #line 126 "C:\Users\pc6vi\source\repos\EA-dotnet-webapi\EADotnetWebapiCli\Templates\Api\Test.tt"

public Element Model { get; set; }
public String ProjectName { get; set; }


        
        #line default
        #line hidden
    }
    
    #line default
    #line hidden
    #region Base class
    /// <summary>
    /// Base class for this transformation
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "17.0.0.0")]
    public class TestBase
    {
        #region Fields
        private global::System.Text.StringBuilder generationEnvironmentField;
        private global::System.CodeDom.Compiler.CompilerErrorCollection errorsField;
        private global::System.Collections.Generic.List<int> indentLengthsField;
        private string currentIndentField = "";
        private bool endsWithNewline;
        private global::System.Collections.Generic.IDictionary<string, object> sessionField;
        #endregion
        #region Properties
        /// <summary>
        /// The string builder that generation-time code is using to assemble generated output
        /// </summary>
        public System.Text.StringBuilder GenerationEnvironment
        {
            get
            {
                if ((this.generationEnvironmentField == null))
                {
                    this.generationEnvironmentField = new global::System.Text.StringBuilder();
                }
                return this.generationEnvironmentField;
            }
            set
            {
                this.generationEnvironmentField = value;
            }
        }
        /// <summary>
        /// The error collection for the generation process
        /// </summary>
        public System.CodeDom.Compiler.CompilerErrorCollection Errors
        {
            get
            {
                if ((this.errorsField == null))
                {
                    this.errorsField = new global::System.CodeDom.Compiler.CompilerErrorCollection();
                }
                return this.errorsField;
            }
        }
        /// <summary>
        /// A list of the lengths of each indent that was added with PushIndent
        /// </summary>
        private System.Collections.Generic.List<int> indentLengths
        {
            get
            {
                if ((this.indentLengthsField == null))
                {
                    this.indentLengthsField = new global::System.Collections.Generic.List<int>();
                }
                return this.indentLengthsField;
            }
        }
        /// <summary>
        /// Gets the current indent we use when adding lines to the output
        /// </summary>
        public string CurrentIndent
        {
            get
            {
                return this.currentIndentField;
            }
        }
        /// <summary>
        /// Current transformation session
        /// </summary>
        public virtual global::System.Collections.Generic.IDictionary<string, object> Session
        {
            get
            {
                return this.sessionField;
            }
            set
            {
                this.sessionField = value;
            }
        }
        #endregion
        #region Transform-time helpers
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void Write(string textToAppend)
        {
            if (string.IsNullOrEmpty(textToAppend))
            {
                return;
            }
            // If we're starting off, or if the previous text ended with a newline,
            // we have to append the current indent first.
            if (((this.GenerationEnvironment.Length == 0) 
                        || this.endsWithNewline))
            {
                this.GenerationEnvironment.Append(this.currentIndentField);
                this.endsWithNewline = false;
            }
            // Check if the current text ends with a newline
            if (textToAppend.EndsWith(global::System.Environment.NewLine, global::System.StringComparison.CurrentCulture))
            {
                this.endsWithNewline = true;
            }
            // This is an optimization. If the current indent is "", then we don't have to do any
            // of the more complex stuff further down.
            if ((this.currentIndentField.Length == 0))
            {
                this.GenerationEnvironment.Append(textToAppend);
                return;
            }
            // Everywhere there is a newline in the text, add an indent after it
            textToAppend = textToAppend.Replace(global::System.Environment.NewLine, (global::System.Environment.NewLine + this.currentIndentField));
            // If the text ends with a newline, then we should strip off the indent added at the very end
            // because the appropriate indent will be added when the next time Write() is called
            if (this.endsWithNewline)
            {
                this.GenerationEnvironment.Append(textToAppend, 0, (textToAppend.Length - this.currentIndentField.Length));
            }
            else
            {
                this.GenerationEnvironment.Append(textToAppend);
            }
        }
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void WriteLine(string textToAppend)
        {
            this.Write(textToAppend);
            this.GenerationEnvironment.AppendLine();
            this.endsWithNewline = true;
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void Write(string format, params object[] args)
        {
            this.Write(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void WriteLine(string format, params object[] args)
        {
            this.WriteLine(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Raise an error
        /// </summary>
        public void Error(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Raise a warning
        /// </summary>
        public void Warning(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            error.IsWarning = true;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Increase the indent
        /// </summary>
        public void PushIndent(string indent)
        {
            if ((indent == null))
            {
                throw new global::System.ArgumentNullException("indent");
            }
            this.currentIndentField = (this.currentIndentField + indent);
            this.indentLengths.Add(indent.Length);
        }
        /// <summary>
        /// Remove the last indent that was added with PushIndent
        /// </summary>
        public string PopIndent()
        {
            string returnValue = "";
            if ((this.indentLengths.Count > 0))
            {
                int indentLength = this.indentLengths[(this.indentLengths.Count - 1)];
                this.indentLengths.RemoveAt((this.indentLengths.Count - 1));
                if ((indentLength > 0))
                {
                    returnValue = this.currentIndentField.Substring((this.currentIndentField.Length - indentLength));
                    this.currentIndentField = this.currentIndentField.Remove((this.currentIndentField.Length - indentLength));
                }
            }
            return returnValue;
        }
        /// <summary>
        /// Remove any indentation
        /// </summary>
        public void ClearIndent()
        {
            this.indentLengths.Clear();
            this.currentIndentField = "";
        }
        #endregion
        #region ToString Helpers
        /// <summary>
        /// Utility class to produce culture-oriented representation of an object as a string.
        /// </summary>
        public class ToStringInstanceHelper
        {
            private System.IFormatProvider formatProviderField  = global::System.Globalization.CultureInfo.InvariantCulture;
            /// <summary>
            /// Gets or sets format provider to be used by ToStringWithCulture method.
            /// </summary>
            public System.IFormatProvider FormatProvider
            {
                get
                {
                    return this.formatProviderField ;
                }
                set
                {
                    if ((value != null))
                    {
                        this.formatProviderField  = value;
                    }
                }
            }
            /// <summary>
            /// This is called from the compile/run appdomain to convert objects within an expression block to a string
            /// </summary>
            public string ToStringWithCulture(object objectToConvert)
            {
                if ((objectToConvert == null))
                {
                    throw new global::System.ArgumentNullException("objectToConvert");
                }
                System.Type t = objectToConvert.GetType();
                System.Reflection.MethodInfo method = t.GetMethod("ToString", new System.Type[] {
                            typeof(System.IFormatProvider)});
                if ((method == null))
                {
                    return objectToConvert.ToString();
                }
                else
                {
                    return ((string)(method.Invoke(objectToConvert, new object[] {
                                this.formatProviderField })));
                }
            }
        }
        private ToStringInstanceHelper toStringHelperField = new ToStringInstanceHelper();
        /// <summary>
        /// Helper to produce culture-oriented representation of an object as a string
        /// </summary>
        public ToStringInstanceHelper ToStringHelper
        {
            get
            {
                return this.toStringHelperField;
            }
        }
        #endregion
    }
    #endregion
}