using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace EADotnetAngularCli
{
    public class EAXmiParser
    {

        static XmlDocument LoadXmlDocumentWithEncoding(string filePath, Encoding encoding)
        {
            // Create a StreamReader with the specified encoding
            using (StreamReader reader = new StreamReader(filePath, encoding))
            {
                // Read the content of the file
                string xmlContent = reader.ReadToEnd();

                // Load the content into an XmlDocument
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlContent);
                return xmlDoc;
            }
        }


        public Element[] Parse(string path)
        {

            var xmlDoc = LoadXmlDocumentWithEncoding(path, Encoding.Latin1);

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

                var attrId = attr.Attributes!["xmi:id"]!.Value;

                var extension = xmlDoc.SelectSingleNode("//xmi:Extension/elements/element[@xmi:idref='" + node.Attributes!["xmi:id"]!.Value + "']/attributes/attribute[@xmi:idref='" + attrId + "']", namespaceManager);

                var tags= extension!.SelectNodes("tags/*", namespaceManager)!.Cast<XmlNode>().ToDictionary(tag => tag.Attributes!["name"]!.Value, tag => tag.Attributes!["value"] != null ? tag.Attributes!["value"]!.Value : "");

                

                var xref = extension!.SelectSingleNode("xrefs", namespaceManager)!.Attributes!["value"]!.Value;
                
                
                var idMatch = Regex.Match(xref, @";\$DES=@PROP=@NAME=isID@ENDNAME;@TYPE=Boolean@ENDTYPE;@VALU=(.*)@ENDVALU;@PRMT=@ENDPRMT;@ENDPROP;");
                var isId = idMatch.Groups[1].Value=="1";


                return new Attribute(attr.Attributes!["name"]!.Value, type, isId, lowerBound, upperBound, stereotypeNode != null ? stereotypeNode.Name : null, tags);
            });

            var stereotypeNode = node.SelectSingleNode("//xmi:XMI/uml:Model/*[@base_Class='"+ node.Attributes!["xmi:id"]!.Value + "']", namespaceManager);
            

            return new Element(node.Attributes!["name"]!.Value, stereotypeNode!=null ? stereotypeNode.Name : null, node.SelectNodes("xmi:Extension/tag", namespaceManager)!.Cast<XmlNode>().ToDictionary(tag => tag.Attributes![0]!.Name, tag => tag.Attributes![0]!.Value), attr.ToArray());
        }



        
    }
}
