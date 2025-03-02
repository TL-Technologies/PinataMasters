using System;
using System.Xml;

// See documentation: https://developer.android.com/training/articles/security-config


//<?xml version="1.0" encoding="utf-8"?>
//<network-security-config>
//    <domain-config>
//        <domain includeSubdomains="true">secure.example.com</domain>
//        <domain includeSubdomains="true">cdn.example.com</domain>
//        <trust-anchors>
//            <certificates src="@raw/trusted_roots"/>
//        </trust-anchors>
//    </domain-config>
//</network-security-config>

namespace Modules.Hive.Editor.BuildUtilities.Android
{
    public class NetworkSecurityConfigResource : IAndroidXmlFile, IDisposable
    {
        const string xmlRootNodeName = "network-security-config";


        /// <inheritdoc/>
        public XmlDocument Xml { get; private set; }


        /// <inheritdoc/>
        public string XmlRootNodeName => xmlRootNodeName;


        /// <summary>
        /// Gets a path to save the file.
        /// </summary>
        public string OutputPath { get; private set; }


        #region Instancing

        /// <summary>
        /// Opens network security config file by path.
        /// </summary>
        /// <param name="path">A path to network security config file.</param>
        public static NetworkSecurityConfigResource Open(string path)
        {
            var xml = new XmlDocument();
            xml.Load(path);

            return new NetworkSecurityConfigResource(path, xml);
        }


        /// <summary>
        /// Creates a new network security config file.
        /// </summary>
        /// <param name="path">Full path to a network security config file.</param>
        public static NetworkSecurityConfigResource Create(string path)
        {
            var xml = new XmlDocument();
            xml.AppendChild(xml.CreateXmlDeclaration("1.0", "utf-8", null));
            xml.AppendChild(xml.CreateElement(xmlRootNodeName));

            return new NetworkSecurityConfigResource(path, xml);
        }


        private NetworkSecurityConfigResource(string path, XmlDocument xml)
        {
            OutputPath = path;
            Xml = xml;
        }


        public void Dispose()
        {
            if (Xml == null)
                return;

            Save();
            Xml = null;
            OutputPath = null;
        }

        #endregion



        #region Common tags management

        public XmlElement GetDomainConfigElement(bool createIfNotExists = false)
            => this.GetNodeByPath("domain-config", createIfNotExists) as XmlElement;


        public XmlElement GetBaseConfigElement(bool createIfNotExists = false)
            => this.GetNodeByPath("base-config", createIfNotExists) as XmlElement;


        public XmlElement GetDebugOverridesElement(bool createIfNotExists = false)
            => this.GetNodeByPath("debug-overrides", createIfNotExists) as XmlElement;


        public XmlElement GetConfigSectionElement(NetworkSecurityConfigSection section, bool createIfNotExists = false)
        {
            switch (section)
            {
                case NetworkSecurityConfigSection.Domain:
                    return GetDomainConfigElement(createIfNotExists);

                case NetworkSecurityConfigSection.Base:
                    return GetBaseConfigElement(createIfNotExists);

                case NetworkSecurityConfigSection.DebugOverrides:
                    return GetDebugOverridesElement(createIfNotExists);

                default:
                    throw new NotImplementedException($"Section '{section}' is not implemented.");
            };
        }

        #endregion



        #region Trusted certificates management

        public XmlElement GetTrustAnchorsElement(NetworkSecurityConfigSection section, bool createIfNotExists = false)
        {
            XmlElement parent = GetConfigSectionElement(section, createIfNotExists);
            XmlElement element = this.FindElement(parent, p => p.LocalName.Equals("trust-anchors", StringComparison.OrdinalIgnoreCase));

            if (createIfNotExists && element == null)
            {
                element = Xml.CreateElement("trust-anchors");
                parent.AppendChild(element);
            }

            return element;
        }


        public void AddTrustedCertificates(NetworkSecurityConfigSection section, params string[] source)
        {
            if (source == null && source.Length == 0)
            {
                return;
            }

            XmlElement parent = GetTrustAnchorsElement(section, true);
            foreach (string src in source)
            {
                if (this.FindElement(parent, p => 
                    p.LocalName.Equals("certificates", StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(p.GetAttribute("src"), src, StringComparison.OrdinalIgnoreCase)) != null)
                    continue;

                XmlElement element = Xml.CreateElement("certificates");
                element.SetAttribute("src", src);
                parent.AppendChild(element);
            }
        }

        #endregion



        #region Cleartext Traffic Permitted

        public bool GetCleartextTrafficPermitted(NetworkSecurityConfigSection section)
        {
            XmlElement element = GetConfigSectionElement(section, false);
            if (element == null)
            {
                // By default
                return false; 
            }

            string value = element.GetAttribute("cleartextTrafficPermitted");
            return bool.Parse(value);
        }


        public void SetCleartextTrafficPermitted(NetworkSecurityConfigSection section, bool value)
        {
            XmlElement element = GetConfigSectionElement(section, true);
            element.SetAttribute("cleartextTrafficPermitted", value ? "true" : "false");
        }

        #endregion
        
        
        
        #region Domain management
        
        public void AddDomain(string domainName, bool includeSubdomains)
        {
            // TODO: add duplicates verification
            XmlElement domainConfigElement = GetDomainConfigElement(true);
            
            XmlElement domainElement = Xml.CreateElement("domain");
            string includeSubdomainsValue = includeSubdomains ? "true" : "false";
            domainElement.SetAttribute("includeSubdomains", includeSubdomainsValue);
            domainElement.InnerText = domainName;
            
            domainConfigElement.AppendChild(domainElement);
        }
        
        #endregion


        public void Save()
        {
            Xml.Save(OutputPath);
        }
    }
}
