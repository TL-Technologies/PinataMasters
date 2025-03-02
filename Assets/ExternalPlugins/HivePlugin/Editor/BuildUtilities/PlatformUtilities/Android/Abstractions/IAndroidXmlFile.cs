using System.Xml;


namespace Modules.Hive.Editor.BuildUtilities.Android
{
    public interface IAndroidXmlFile
    {
        /// <summary>
        /// Gets an xml document.
        /// </summary>
        XmlDocument Xml { get; }

        /// <summary>
        /// Gets a tag-name of xml root node.
        /// </summary>
        string XmlRootNodeName { get; }
    }
}
