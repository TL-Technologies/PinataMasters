using System;
using System.Collections.Generic;
using System.Xml;


namespace Modules.Hive.Editor.BuildUtilities.Android
{
    public static class AndroidXmlFileExtensions
    {
        #region Nodes

        public static XmlNode GetRootNode(this IAndroidXmlFile xmlFile, bool createIfNotExists = true)
        {
            return xmlFile.GetNodeByPath(null, createIfNotExists);
        }


        public static XmlNode GetNodeByPath(this IAndroidXmlFile xmlFile, string path, bool createIfNotExists = false)
        {
            XmlDocument xml = xmlFile.Xml;            

            path = string.IsNullOrEmpty(path) ? xmlFile.XmlRootNodeName : $"{xmlFile.XmlRootNodeName}/{path}";
            string[] keys = path.Split('/');
            XmlNode node = xml.FirstChild;
            XmlNode parent = node;

            int i = 0;
            while (node != null)
            {
                if (node.Name == keys[i])
                {
                    if (++i == keys.Length)
                    {
                        break;
                    }

                    parent = node;
                    node = node.FirstChild;
                }
                else
                {
                    node = node.NextSibling;
                }
            }

            if (node == null && createIfNotExists)
            {
                node = parent;
                while (i < keys.Length)
                {
                    node = node.AppendChild(xml.CreateElement(keys[i++]));
                }
            }

            return node;
        }

        #endregion

        
        
        #region Elements

        public static XmlElement FindElement(this IAndroidXmlFile xmlFile, XmlNode root, Predicate<XmlElement> predicate)
        {
            if (predicate == null)
            {
                return null;
            }

            root = root != null ? root.FirstChild : xmlFile.GetRootNode().FirstChild;
            if (root == null)
            {
                return null;
            }

            while (true)
            {
                if (root == null)
                {
                    return null;
                }

                if (root.NodeType == XmlNodeType.Element && predicate(root as XmlElement))
                {
                    return root as XmlElement;
                }

                root = root.NextSibling;
            }
        }


        public static XmlElement FindElement(this IAndroidXmlFile xmlFile, Predicate<XmlElement> predicate)
        {
            return FindElement(xmlFile, null, predicate);
        }


        public static XmlElement[] FindElements(this IAndroidXmlFile _, XmlNode root, Predicate<XmlElement> predicate)
        {
            if (predicate == null)
            {
                return null;
            }

            List<XmlElement> rs = new List<XmlElement>();
            root = root.FirstChild;
            while (true)
            {
                if (root == null)
                {
                    return rs.ToArray();
                }

                if (root.NodeType == XmlNodeType.Element && predicate(root as XmlElement))
                {
                    rs.Add(root as XmlElement);
                }

                root = root.NextSibling;
            }
        }

        #endregion



        #region Attributes

        public static XmlAttribute FindAttribute(this IAndroidXmlFile _, XmlNode node, Predicate<XmlAttribute> predicate)
        {
            if (node.Attributes == null || predicate == null)
            {
                return null;
            }

            for (int i = 0; i < node.Attributes.Count; i++)
            {
                if (predicate(node.Attributes[i]))
                {
                    return node.Attributes[i];
                }
            }

            return null;
        }


        public static XmlAttribute FindAttribute(this IAndroidXmlFile _, XmlNode node, string attributeName)
        {
            if (node.Attributes == null)
            {
                return null;
            }

            for (int i = 0; i < node.Attributes.Count; i++)
            {
                if (node.Attributes[i].LocalName == attributeName)
                {
                    return node.Attributes[i];
                }
            }

            return null;
        }


        public static XmlAttribute SetAttribute(this IAndroidXmlFile xmlFile, XmlNode node, string attributeName, string value)
        {
            XmlAttribute attr = xmlFile.FindAttribute(node, attributeName);
            if (attr == null)
            {
                attr = xmlFile.Xml.CreateAttribute(attributeName);
                node.Attributes.Append(attr);
            }

            attr.Value = value;
            return attr;
        }


        public static bool IsAttributeValueEquals(this IAndroidXmlFile xmlFile, XmlNode node, string attributeName, string value)
        {
            XmlAttribute attr = xmlFile.FindAttribute(node, attributeName);
            return attr != null && value.Equals(attr.Value);
        }

        #endregion
    }
}
