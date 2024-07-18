using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EADotnetAngularCli
{
    public class Element
    {
        public string Name { get; set; }
        public string? Stereotype { get; set; }
        public Dictionary<string, string> Tags { get; set; }
        public Attribute[] Attributes { get; set; }

        // contructor
        public Element(string name, string? stereotype, Dictionary<string, string> tags, Attribute[] attributes)
        {
            Name = name;
            Stereotype = stereotype;
            Tags = tags;
            Attributes = attributes;
        }


        public bool IsPrimitive
        {
            get
            {
                return Name.StartsWith("EAC__");
            }
        }
    }


    public class Attribute
    {
        public string Name { get; set; }
        public Element Type { get; set; }
        public bool IsId { get; set; }
        public string LowerBound { get; set; }
        public string UpperBound { get; set; }

        public string? Stereotype { get; set; }

        public Dictionary<string, string> Tags { get; set; }

        //constructor
        public Attribute(string name, Element type, bool isId, string lowerBound, string upperBound, string? stereotype, Dictionary<string, string> tags)
        {
            Name = name;
            Type = type;
            IsId = isId;
            LowerBound = lowerBound;
            UpperBound = upperBound;
            Stereotype = stereotype;
            Tags = tags;
        }

        public bool IsNullable
        {
            get
            {
                return LowerBound == "0";
            }
        }
    }

}
