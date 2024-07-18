﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 17.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace EADotnetAngularCli.Templates.Client
{
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;
    using CaseExtensions;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Users\David\source\repos\EA-dotnet-angular\EADotnetAngularCli\Templates\Client\Stories.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "17.0.0.0")]
    public partial class Stories : StoriesBase
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public virtual string TransformText()
        {
            this.Write("import { moduleMetadata, type Meta, type StoryObj } from \'@storybook/angular\';\r\ni" +
                    "mport { ");
            
            #line 8 "C:\Users\David\source\repos\EA-dotnet-angular\EADotnetAngularCli\Templates\Client\Stories.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Model.Name));
            
            #line default
            #line hidden
            this.Write("ListComponent } from \'../app/");
            
            #line 8 "C:\Users\David\source\repos\EA-dotnet-angular\EADotnetAngularCli\Templates\Client\Stories.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Model.Name.ToKebabCase()));
            
            #line default
            #line hidden
            this.Write("-list/");
            
            #line 8 "C:\Users\David\source\repos\EA-dotnet-angular\EADotnetAngularCli\Templates\Client\Stories.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Model.Name.ToKebabCase()));
            
            #line default
            #line hidden
            this.Write(@"-list.component';
import { Api } from '../api';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule, provideAnimations } from '@angular/platform-browser/animations';
import { MatIconModule } from '@angular/material/icon';
import { applicationConfig } from '@storybook/angular';
import { importProvidersFrom } from '@angular/core';

// More on how to set up stories at: https://storybook.js.org/docs/writing-stories
const meta: Meta<");
            
            #line 17 "C:\Users\David\source\repos\EA-dotnet-angular\EADotnetAngularCli\Templates\Client\Stories.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Model.Name));
            
            #line default
            #line hidden
            this.Write("ListComponent> = {\r\n  title: \'");
            
            #line 18 "C:\Users\David\source\repos\EA-dotnet-angular\EADotnetAngularCli\Templates\Client\Stories.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Model.Name));
            
            #line default
            #line hidden
            this.Write("/List\',\r\n  component: ");
            
            #line 19 "C:\Users\David\source\repos\EA-dotnet-angular\EADotnetAngularCli\Templates\Client\Stories.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Model.Name));
            
            #line default
            #line hidden
            this.Write(@"ListComponent,
  decorators: [
    moduleMetadata({
      providers: [
       
      ],
      imports: [MatIconModule]
    }),
      applicationConfig({
        providers: [
          // Use `provideAnimations` if your Angular version supports it
          provideAnimations(),
          BrowserModule,
          // If `provideAnimations` is not supported, use `importProvidersFrom`
          importProvidersFrom(BrowserAnimationsModule),
          { provide: Api, useFactory: () => new Api({ baseUrl: 'http://localhost:6006/api', customFetch: fetch }) }
        ],
      }),
    
  ]
  , parameters: {
    layout: 'centered',
  }
};

export default meta;
type Story = StoryObj<");
            
            #line 45 "C:\Users\David\source\repos\EA-dotnet-angular\EADotnetAngularCli\Templates\Client\Stories.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Model.Name));
            
            #line default
            #line hidden
            this.Write("ListComponent>;\r\n\r\n\r\nexport const Default: Story = {\r\n\r\n  \r\n\r\n};\r\n\r\nexport const " +
                    "NonValid: Story = {\r\n\r\n  parameters: {\r\n    mockData: [\r\n      {\r\n        url: \'" +
                    "http://localhost:6006/api/");
            
            #line 59 "C:\Users\David\source\repos\EA-dotnet-angular\EADotnetAngularCli\Templates\Client\Stories.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Model.Name));
            
            #line default
            #line hidden
            this.Write("\',\r\n        method: \'PUT\',\r\n        status: 400,\r\n        response:\r\n          {\"" +
                    "type\":\"https://tools.ietf.org/html/rfc9110#section-15.5.1\",\"title\":\"One or more " +
                    "validation errors occurred.\",\"status\":400,\"errors\": {");
            
            #line 63 "C:\Users\David\source\repos\EA-dotnet-angular\EADotnetAngularCli\Templates\Client\Stories.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(string.Join(", ", Model.Attributes.Where(x=>x.IsNullable).Select(x=>"\""+x.Name+"\":[\"The "+x.Name+" field is required.\"]"))));
            
            #line default
            #line hidden
            this.Write("},\"traceId\":\"00-f58504593e134cfa3e926ec858fffa4f-f48fc68a96ebb8b4-00\"},\r\n      }," +
                    "\r\n    ],\r\n  }\r\n\r\n};\r\n\r\n\r\n\r\n");
            return this.GenerationEnvironment.ToString();
        }
        
        #line 72 "C:\Users\David\source\repos\EA-dotnet-angular\EADotnetAngularCli\Templates\Client\Stories.tt"

public Element Model { get; set; }



        
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
    public class StoriesBase
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
