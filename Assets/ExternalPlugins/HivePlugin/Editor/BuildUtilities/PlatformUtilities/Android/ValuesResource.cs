using System;
using System.Xml;


namespace Modules.Hive.Editor.BuildUtilities.Android
{
    public class ValuesResource : IAndroidXmlFile, IDisposable
    {
        /// <inheritdoc/>
        public XmlDocument Xml { get; private set; }


        /// <inheritdoc/>
        public string XmlRootNodeName => "resources";


        /// <summary>
        /// Gets a path to save the file.
        /// </summary>
        public string OutputPath { get; private set; }


        #region Instancing

        /// <summary>
        /// Opens values resource file by path.
        /// </summary>
        /// <param name="path">A path to values resource file.</param>
        public static ValuesResource Open(string path)
        {
            var xml = new XmlDocument();
            xml.Load(path);

            return new ValuesResource(path, xml);
        }


        /// <summary>
        /// Creates a new values resource file.
        /// </summary>
        /// <param name="path">Full path to a values resource file.</param>
        public static ValuesResource Create(string path)
        {
            var xml = new XmlDocument();
            xml.LoadXml($"<resources></resources>");

            return new ValuesResource(path, xml);
        }


        private ValuesResource(string path, XmlDocument xml)
        {
            OutputPath = path;
            Xml = xml;
        }


        public void Dispose()
        {
            if (Xml == null)
            {
                return;
            }

            Save();
            Xml = null;
            OutputPath = null;
        }

        #endregion



        #region Xml tools

        public XmlElement GetKeyValueElement(string elementName, string key, bool createIfNotExists = false)
        {
            XmlNode root = this.GetRootNode();
            XmlNode node = root.FirstChild;
            while (node != null)
            {
                if (node.NodeType == XmlNodeType.Element && 
                    node.Name == elementName && 
                    this.IsAttributeValueEquals(node, "name", key))
                {
                    break;
                }

                node = node.NextSibling;
            }

            if (node == null && createIfNotExists)
            {
                node = Xml.CreateElement(elementName);
                this.SetAttribute(node, "name", key);
                root.AppendChild(node);
            }

            return node as XmlElement;
        }

        #endregion



        #region Public properties and methods

        public void SetString(string key, string value)
        {
            XmlElement elem = GetKeyValueElement("string", key, true);
            elem.InnerText = value;
        }


        public string GetString(string key, string defaultValue = null)
        {
            XmlElement elem = GetKeyValueElement("string", key);
            return elem != null ? elem.InnerText : defaultValue;
        }


        public void Save()
        {
            Xml.Save(OutputPath);
        }

        #endregion
    }
}
