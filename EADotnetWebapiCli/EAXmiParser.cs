using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EADotnetWebapiCli
{
    public class EAXmiParser
    {


        public Element[] Parse(string path)
        {

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(File.ReadAllText(path));

            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(xmlDoc.NameTable);
            namespaceManager.AddNamespace("xmi", "http://schema.omg.org/spec/XMI/2.1");
            namespaceManager.AddNamespace("uml", "http://schema.omg.org/spec/UML/2.1");

            return xmlDoc.SelectNodes("//xmi:XMI/uml:Model/packagedElement/packagedElement[@xmi:type='uml:Class']", namespaceManager)!.Cast<XmlNode>().Select(x => GetUmlClass(x, xmlDoc, namespaceManager)).ToArray();
        }


        Element GetUmlClass(XmlNode node, XmlDocument xmlDoc, XmlNamespaceManager namespaceManager)
        {
            var attr = node.SelectNodes("ownedAttribute", namespaceManager)!.Cast<XmlNode>().Select(attr =>
            {
                var classes = xmlDoc.SelectNodes("//xmi:XMI/uml:Model/packagedElement/packagedElement[@xmi:type='uml:Class']", namespaceManager)!.Cast<XmlNode>().ToDictionary(x => x.Attributes!["xmi:id"]!.Value, x => x);


                var isPrimitive = false;
                
                var typeAttr = attr.SelectSingleNode("type")!.Attributes!["xmi:idref"];

                isPrimitive = typeAttr!.Value.Trim().StartsWith("EAC__");


                string? typeId = attr.SelectSingleNode("type")!.Attributes!["xmi:idref"]!.Value;


                var isId = attr.Attributes!["isID"]?.Value == "true";
                

                var lowerBoundNode = attr.SelectNodes("lowerValue");

                var lowerBound = "1";
                if (lowerBoundNode!.Count > 0)
                {
                    lowerBound = lowerBoundNode[0]!.Attributes!["value"]?.Value ?? "1";
                }


                var upperBoundNode = attr.SelectNodes("upperValue");
                var upperBound = "1";
                if (upperBoundNode!.Count > 0)
                {
                    upperBound = upperBoundNode[0]!.Attributes!["value"]?.Value ?? "1";
                }


                var type = isPrimitive ? new Element(typeId, null, new Dictionary<string, string>(), Array.Empty<Attribute>()) : GetUmlClass(classes[typeId], xmlDoc, namespaceManager);

                var stereotypeNode = node.SelectSingleNode("//xmi:XMI/uml:Model/*[@base_Property='" + attr.Attributes!["xmi:id"]!.Value + "']", namespaceManager);



                return new Attribute(attr.Attributes!["name"]!.Value, type, isId, lowerBound, upperBound, stereotypeNode!=null ? stereotypeNode.Name : null);
            });

            var stereotypeNode = node.SelectSingleNode("//xmi:XMI/uml:Model/*[@base_Class='"+ node.Attributes!["xmi:id"]!.Value + "']", namespaceManager);
            

            return new Element(node.Attributes!["name"]!.Value, stereotypeNode!=null ? stereotypeNode.Name : null, node.SelectNodes("xmi:Extension/tag", namespaceManager)!.Cast<XmlNode>().ToDictionary(tag => tag.Attributes![0]!.Name, tag => tag.Attributes![0]!.Value), attr.ToArray());
        }



        
    }
}
