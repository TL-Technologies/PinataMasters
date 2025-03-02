using System;
using System.Collections.Generic;
using System.Xml;


// Documentation: https://developer.android.com/guide/topics/data/autobackup?utm_campaign=autobackup-729&utm_medium=blog&utm_source=dac#configuring
namespace Modules.Hive.Editor.BuildUtilities.Android
{
    public class BackupRulesResource : IAndroidXmlFile, IDisposable
    {
        private const string RootNodeName = "full-backup-content";
        private HashSet<string> includedDomains = new HashSet<string>();


        /// <inheritdoc/>
        public XmlDocument Xml { get; private set; }


        /// <inheritdoc/>
        public string XmlRootNodeName => RootNodeName;


        /// <summary>
        /// Gets a path to save the file.
        /// </summary>
        public string OutputPath { get; private set; }


        #region Instancing

        /// <summary>
        /// Opens backup rules file by path.
        /// </summary>
        /// <param name="path">A path to backup rules file.</param>
        public static BackupRulesResource Open(string path)
        {
            var xml = new XmlDocument();
            xml.Load(path);

            return new BackupRulesResource(path, xml);
        }


        /// <summary>
        /// Creates a new backup rules file.
        /// </summary>
        /// <param name="path">Full path to a backup rules file.</param>
        public static BackupRulesResource Create(string path)
        {
            var xml = new XmlDocument();
            xml.AppendChild(xml.CreateElement(RootNodeName));

            return new BackupRulesResource(path, xml);
        }


        private BackupRulesResource(string path, XmlDocument xml)
        {
            OutputPath = path;
            Xml = xml;

            // Looking for configured domains to keep default rules
            if (xml.HasChildNodes)
            {
                foreach (var node in xml.ChildNodes)
                {
                    if (node is XmlElement element)
                    {
                        string value = element.GetAttribute("path");
                        if (!string.IsNullOrEmpty(value))
                        {
                            includedDomains.Add(element.Name);
                        }
                    }
                }
            }
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


        #region Common tools

        private XmlElement CreateElement(AndroidBackupRuleType ruleType)
        {
            switch (ruleType)
            {
                case AndroidBackupRuleType.Include:
                    return Xml.CreateElement("include");

                case AndroidBackupRuleType.Exclude:
                    return Xml.CreateElement("exclude");

                default:
                    throw new NotImplementedException($"Backup rule type '{ruleType}' is not implemented.");
            }
        }


        private string GetDomainString(AndroidBackupDomain domain)
        {
            switch (domain)
            {
                case AndroidBackupDomain.File:
                    return "file";

                case AndroidBackupDomain.Database:
                    return "database";

                case AndroidBackupDomain.SharedPref:
                    return "sharedpref";

                case AndroidBackupDomain.External:
                    return "external";

                case AndroidBackupDomain.Root:
                    return "root";

                default:
                    throw new NotImplementedException($"Backup domain '{domain}' is not implemented.");
            }
        }


        /// <summary>
        /// Adds a new backup rule.
        /// </summary>
        /// <param name="rule">The rule to add.</param>
        /// <returns>Xml element that describes the rule.</returns>
        public XmlElement AddBackupRule(AndroidBackupRule rule)
        {
            XmlElement element = null;
            string domain = GetDomainString(rule.domain);

            // Checks whether it's need to add a default rule
            if (!includedDomains.Contains(domain))
            {
                element = CreateElement(AndroidBackupRuleType.Include);
                element.SetAttribute("domain", domain);
                element.SetAttribute("path", ".");

                this.GetRootNode().AppendChild(element);
                includedDomains.Add(domain);
            }

            // Add new rule
            element = CreateElement(rule.type);
            element.SetAttribute("domain", domain);
            element.SetAttribute("path", rule.path);

            this.GetRootNode().AppendChild(element);

            return element;
        }


        /// <summary>
        /// Adds a new backup rule.
        /// </summary>
        /// <param name="ruleType">A type of the rule to add.</param>
        /// <param name="domain">Domain of the rule.</param>
        /// <param name="path">Path to a file.</param>
        /// <returns>Xml element that describes the rule.</returns>
        public XmlElement AddBackupRule(AndroidBackupRuleType ruleType, AndroidBackupDomain domain, string path)
        {
            return AddBackupRule(new AndroidBackupRule(ruleType, domain, path));
        }


        public void Save()
        {
            Xml.Save(OutputPath);
        }

        #endregion
    }
}
