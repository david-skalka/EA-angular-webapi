using Pluralize.NET.Core;

namespace EADotnetAngularCli
{


    public static class StringPluralizerExtension
    {
        public static string Pluralize(this string str)
        {
            return new Pluralizer().Pluralize(str);
        }
    }
   
    
}
