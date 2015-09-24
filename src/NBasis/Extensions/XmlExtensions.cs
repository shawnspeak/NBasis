using System;
using System.Xml.Linq;

namespace NBasis
{
    public static class XmlExtensions
    {
        public static bool HasAttribute(this XElement node, String attrName)
        {
            return ((!String.IsNullOrEmpty(attrName)) && (node != null) && (node.Attribute(XName.Get(attrName)) != null));
        }

        public static String GetAttributeValue(this XElement node, String attrName)
        {
            return GetAttributeValue(node, attrName, String.Empty);
        }

        public static String GetAttributeValue(this XElement node, String attrName, String defaultValue)
        {
            if (node.HasAttribute(attrName))
                return node.Attribute(XName.Get(attrName)).Value;
            return defaultValue;
        }

        public static bool HasElement(this XElement node, String elementName)
        {
            return ((!String.IsNullOrEmpty(elementName)) && (node != null) && (node.Element(XName.Get(elementName)) != null));
        }

        public static String GetElementValue(this XElement node, String elementName)
        {
            return node.GetElementValue(elementName, String.Empty);
        }

        public static String GetElementValue(this XElement node, String elementName, String defaultValue)
        {
            if (node.HasElement(elementName))
                return node.Element(XName.Get(elementName)).Value;
            return defaultValue;
        }
    }
}
