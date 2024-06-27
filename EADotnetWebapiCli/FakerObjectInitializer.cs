using AutoBogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EADotnetWebapiCli
{
    internal class FakerObjectInitializer
    {

        private Element _model { get; set; }

        private Dictionary<string, object> _override { get; set; }


        Dictionary<Type, Func<object, string>> _ValueFormaters = new() {
            { typeof(string), (value) => "\"" + ((string)value) + "\"" },
            { typeof(int), (value) => ((int)value).ToString() },
            { typeof(bool), (value) => ((bool)value) ? "true" : "false"},
            { typeof(decimal), (value) => ((decimal)value).ToString()+ "m" }
        };


        public FakerObjectInitializer(Element model, Dictionary<string, object> _override)
        {
            _model = model;
            this._override = _override;
        }

        public override string ToString()
        {
            return "new " + _model.Name + "() { " + string.Join(", ", GetData(_model.Attributes, _override).Select(x => x.Key + '=' + _ValueFormaters[x.Value.GetType()](x.Value))) + " }";
        }

        private Dictionary<string, object> GetData(Attribute[] attributes, Dictionary<string, object> _override)
        {
            var retD = new Dictionary<string, object>();

            foreach (var attr in attributes)
            {
                if (attr.Type.Stereotype == "Primitive")
                {
                    var fakeValue = getFakeValue(attr.Type.Name);
                    retD.Add(attr.Name, fakeValue);
                }
                else
                {
                    retD.Add(attr.Name+"Id", AutoFaker.Generate<int>());
                }
            }

            
            foreach (var item in _override)
            {
                retD[item.Key] = item.Value;
            }

            return retD;
        }


        object getFakeValue(string type)
        {


            switch (type)
            {
                case "EAC__String":
                    return AutoFaker.Generate<string>();
                case "EAC__int":
                    return AutoFaker.Generate<int>();
                case "EAC__Boolean":
                    return AutoFaker.Generate<bool>();
                case "EAC__Decimal":
                    return AutoFaker.Generate<decimal>();
                default:
                    throw new NotImplementedException();
            }


        }

    }
}
