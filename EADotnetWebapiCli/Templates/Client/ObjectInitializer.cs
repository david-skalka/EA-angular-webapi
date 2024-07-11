using CaseExtensions;
using EADotnetWebapiCli.Templates.Api;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EADotnetWebapiCli.Templates.Client
{
    public class ObjectInitializer
    {


        public Element model;
        public Dictionary<string, object?> values;

        
        public ObjectInitializer(Element model, Dictionary<string, object?> values)
        {
            this.model = model;
            this.values = values;
        }

        Dictionary<string, Func<object, string>> _valueFormaters = new() {
            { "EAC__String", (value) => "\"" + ((string)value) + "\"" },
            { "EAC__int", (value) => ((int)value).ToString() },
            { "EAC__Boolean", (value) => ((bool)value) ? "true" : "false"},
            { "EAC__Decimal", (value) => ((decimal)value).ToString(new CultureInfo("en-US")) }
};

        public string ToText()
        {
            return "{ " + string.Join(", ", model.Attributes.Where(x => x.Type.IsPrimitive).Where(x => values[x.Name] != null).Select(x => x.Name.ToCamelCase() + ':' + _valueFormaters[x.Type.Name](values[x.Name]!))) + " }";
        }

    }
}
