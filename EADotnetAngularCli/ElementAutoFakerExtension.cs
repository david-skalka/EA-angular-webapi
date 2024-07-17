using AutoBogus;

namespace EADotnetAngularCli
{


    public static class ElementAutoFaker
    {
        public static Dictionary<string, object> GenerateFromElement(Element el)
        {
            var retD = new Dictionary<string, object>();

            foreach (var attr in el.Attributes)
            {
                if (attr.Type.IsPrimitive)
                {
                    var fakeValue = GetFakeValue(attr.Type.Name);
                    retD.Add(attr.Name, fakeValue);
                }
                else
                {
                    retD.Add(attr.Name + "Id", AutoFaker.Generate<int>());
                }
            }
            

            return retD;
        }


      

        static object GetFakeValue(string type)
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
