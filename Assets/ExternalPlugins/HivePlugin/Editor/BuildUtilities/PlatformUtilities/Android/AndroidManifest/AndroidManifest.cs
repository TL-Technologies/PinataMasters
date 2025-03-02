using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using UnityEngine;


namespace Modules.Hive.Editor.BuildUtilities.Android
{
    public class AndroidManifest : IAndroidXmlFile, IDisposable
    {
        public const string PackageRootName = "org.hive";
        public const string DefaultPackageName = "${applicationId}"; // doesn't work in Assets/Plugins/Android

        /// <summary>
        /// Gets an xml document of the manifest.
        /// </summary>
        public XmlDocument Xml { get; private set; }


        /// <summary>
        /// Gets a tag-name of xml root node of the manifest.
        /// </summary>
        public string XmlRootNodeName => "manifest";


        /// <summary>
        /// Gets a path to save the manifest.
        /// </summary>
        public string OutputPath { get; private set; }


        #region Instancing

        /// <summary>
        /// Opens a manifest by path.
        /// </summary>
        /// <param name="path">A path to manifest.</param>
        public static AndroidManifest Open(string path)
        {
            var manifest = new XmlDocument();
            manifest.Load(path);

            return new AndroidManifest(path, manifest);
        }


        /// <summary>
        /// Creates a new android manifest file.
        /// </summary>
        /// <param name="path">Target path to manifest, include file name.</param>
        /// <param name="package">A package name of the manifest.</param>
        public static AndroidManifest Create(string path, string package = null)
        {
            if (string.IsNullOrEmpty(package))
            {
                package = GeneratePackageName();
            }

            var manifest = new XmlDocument();
            manifest.LoadXml($"<?xml version=\"1.0\" encoding=\"utf-8\"?><manifest xmlns:android=\"http://schemas.android.com/apk/res/android\" package=\"{package}\"><application></application></manifest>");

            return new AndroidManifest(path, manifest);
        }


        private AndroidManifest(string path, XmlDocument manifest)
        {
            OutputPath = path;
            Xml = manifest;
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



        #region Xml namespaces management

        private string androidNamespace = null;
        private string toolsNamespace = null;


        /// <summary>
        /// Gets the default namespace URI for "android" prefix.
        /// </summary>
        public string AndroidNamespace => androidNamespace ?? (androidNamespace = GetNamespaceOfPrefix("android"));


        /// <summary>
        /// Gets the namespace URI for "tools" prefix.
        /// </summary>
        public string AndroidToolsNamespace => toolsNamespace ?? (toolsNamespace = GetOrCreateNamespaceOfPrefix("tools", "http://schemas.android.com/tools"));


        /// <summary>
        /// Gets whether a namespace URI is defined for the given prefix that is in scope for the specified node.
        /// </summary>
        /// <param name="node">The node that defines a scope. Can be null that means a scope for the root node.</param>
        /// <param name="prefix">The prefix you want to check.</param>
        /// <returns>True if the namespace is defined; otherwise, false.</returns>
        public bool HasNamespaceOfPrefix(XmlElement node, string prefix)
        {
            return !string.IsNullOrEmpty(node.GetNamespaceOfPrefix(prefix));
        }


        /// <summary>
        /// Gets whether a namespace URI is defined for the given prefix that is in scope for the root node.
        /// </summary>
        /// <param name="prefix">The prefix you want to check.</param>
        /// <returns>True if the namespace is defined; otherwise, false.</returns>
        public bool HasNamespaceOfPrefix(string prefix)
        {
            return HasNamespaceOfPrefix(null, prefix);
        }


        /// <summary>
        /// Sets an xmlns declaration for the given prefix that is in scope for the specified node.
        /// </summary>
        /// <param name="node">The node that defines a scope. Can be null that means a scope for the root node.</param>
        /// <param name="prefix">The prefix you want to define.</param>
        /// <param name="namespaceUri">The namespace URI of the specified prefix.</param>
        public void SetNamespaceUri(XmlElement node, string prefix, string namespaceUri)
        {
            if (node == null)
            {
                node = this.GetRootNode() as XmlElement;
            }

            node.SetAttribute($"xmlns:{prefix}", namespaceUri);
        }


        /// <summary>
        /// Sets an xmlns declaration for the given prefix that is in scope for the root node.
        /// </summary>
        /// <param name="prefix">The prefix you want to define.</param>
        /// <param name="namespaceUri">The namespace URI of the specified prefix.</param>
        public void SetNamespaceUri(string prefix, string namespaceUri)
        {
            SetNamespaceUri(null, prefix, namespaceUri);
        }


        /// <summary>
        /// Looks up the closest xmlns declaration for the given prefix that is in scope for the specified node 
        /// and returns the namespace URI in the declaration.
        /// </summary>
        /// <param name="node">The node that defines a scope. Can be null that means a scope for the root node.</param>
        /// <param name="prefix">The prefix whose namespace URI you want to find.</param>
        /// <returns>The namespace URI of the specified prefix.</returns>
        public string GetNamespaceOfPrefix(XmlElement node, string prefix)
        {
            if (node == null)
            {
                node = this.GetRootNode() as XmlElement;
            }

            return node.GetNamespaceOfPrefix(prefix);
        }


        /// <summary>
        /// Looks up the closest xmlns declaration for the given prefix that is in scope for the root node 
        /// and returns the namespace URI in the declaration.
        /// </summary>
        /// <param name="prefix">The prefix whose namespace URI you want to find.</param>
        /// <returns>The namespace URI of the specified prefix.</returns>
        public string GetNamespaceOfPrefix(string prefix)
        {
            return GetNamespaceOfPrefix(null, prefix);
        }


        /// <summary>
        /// Looks up the closest xmlns declaration for the given prefix that is in scope for the specified node 
        /// or create it if doesn't exist, and returns the namespace URI in the declaration.
        /// </summary>
        /// <param name="prefix">The prefix whose namespace URI you want to find.</param>
        /// <param name="namespaceUri">The namespace URI that will be added if prefix doesn't exist.</param>
        /// <returns></returns>
        private string GetOrCreateNamespaceOfPrefix(string prefix, string namespaceUri)
        {
            string ns = GetNamespaceOfPrefix(null, prefix);
            if (string.IsNullOrEmpty(ns))
            {
                SetNamespaceUri(prefix, namespaceUri);
                return namespaceUri;
            }

            if (namespaceUri.Equals(ns))
            {
                return namespaceUri;
            }

            throw new ArgumentException($"The namespace with prefix '{prefix}' already exists and its URI '{ns}' cannot be overwritten with other URI '{namespaceUri}'.");
        }

        #endregion
        
        
        
        #region Library management
        
        /// <summary>
        /// Gets a uses-library element with specified library name or null if it doesn't exist.
        /// </summary>
        /// <param name="libraryName">The library name to get.</param>
        /// <returns>The library xml element with specified requirements.</returns>
        public XmlElement GetLibraryElement(string libraryName)
        {
            return this.FindElement(element => 
                element.LocalName == "uses-library" && 
                this.IsAttributeValueEquals(element, "name", libraryName));
        }
        
        
        /// <summary>
        /// Adds uses-library element with specified name and requirements if it doesn't exist.
        /// Anyway it returns an instance of the element.
        /// </summary>
        /// <param name="name">The name of the element to add.</param>
        /// <param name="isRequired">Indicates whether the application requires the library specified by name.</param>
        /// <returns>The library xml element with specified requirements.</returns>
        public XmlElement AddLibraryElement(string name, bool isRequired)
        {
            XmlElement element = GetLibraryElement(name);
            if (element == null)
            {
                element = Xml.CreateElement("uses-library");

                element.SetAttribute("name", AndroidNamespace, name);
                element.SetAttribute("required", AndroidNamespace, isRequired ? "true" : "false");
                element.IsEmpty = true;
                
                XmlNode node = GetApplicationElement(true);
                node.AppendChild(element);
            }
            else if (isRequired)
            {
                element.SetAttribute("required", AndroidNamespace, "true");
            }

            return element;
        }
        
        #endregion



        #region Permissions management

        /// <summary>
        /// Gets a uses-permission element with specified permission or null if it doesn't exist.
        /// </summary>
        /// <param name="permission">The permission of the element to get.</param>
        /// <returns>The xml element with specified permission.</returns>
        public XmlElement GetPermissionElement(Permission permission)
        {
            return this.FindElement(element => element.LocalName == "uses-permission" && 
                                        this.IsAttributeValueEquals(element, "name", permission.Key));
        }


        /// <summary>
        /// Adds uses-permission element with specified permission if it doesn't exist.
        /// Anyway it returns an instance of the element.
        /// </summary>
        /// <param name="permission">The permission of the element to add.</param>
        /// <returns>The xml element with specified permission.</returns>
        public XmlElement AddPermissionElement(Permission permission)
        {
            XmlElement element = AddPermissionElementInternal(permission);

            // Show warning if the permission is flagged as dangerous
            if (permission.IsDangerous)
            {
                Debug.Log($"The permission '{permission.Key}' is marked as dangerous and should be excluded from build if it possible.");
            }
            
            return element;
        }
        

        /// <summary>
        /// Add uses-permission element with specified permission and mark as removed if it doesn't exist.
        /// if element exists, it's marked as removed.
        /// </summary>
        /// <param name="permission">The permission of the element to remove</param>
        public void ForceRemovingPermissionElement(Permission permission)
        {
            XmlElement element = AddPermissionElementInternal(permission);
            element.SetAttribute("node", AndroidToolsNamespace, "remove");
        }
        

        /// <summary>
        /// Gets whether the manifest contains uses-permission element with specified permission.
        /// </summary>
        /// <param name="permission">The permission of the element.</param>
        /// <returns>True, if element exists; otherwise - false.</returns>
        public bool HasPermissionElement(Permission permission)
        {
            return GetPermissionElement(permission) != null;
        }

        
        /// <summary>
        /// Adds uses-permission element with specified permission if it doesn't exist.
        /// Anyway it returns an instance of the element.
        /// </summary>
        /// <param name="permission">The permission of the element to add.</param>
        /// <returns>The xml element with specified permission.</returns>
        private XmlElement AddPermissionElementInternal(Permission permission)
        {
            XmlElement element = GetPermissionElement(permission);
            if (element == null)
            {
                XmlNode node = this.GetRootNode();
                element = Xml.CreateElement("uses-permission");
                element.SetAttribute("name", AndroidNamespace, permission.Key);
                element.IsEmpty = true;
                node.AppendChild(element);
            }

            return element;
        }

        #endregion



        #region Compatible screens management

        public XmlElement GetCompatibleScreensElement(bool createIfNotExists = false) 
            => this.GetNodeByPath("compatible-screens", createIfNotExists) as XmlElement;


        public HashSet<CompatibleScreen> GetCompatibleScreens()
        {
            //<compatible-screens>
            //    <screen android:screenSize="small" android:screenDensity="ldpi" />
            //    <screen android:screenSize="small" android:screenDensity="mdpi" />
            //    ...
            //<application>

            XmlElement element = this.GetNodeByPath("compatible-screens", false) as XmlElement;
            HashSet<CompatibleScreen> screens = new HashSet<CompatibleScreen>();

            if (element != null)
            {
                foreach (var node in element.ChildNodes)
                {
                    XmlElement screenElement = node as XmlElement;
                    screens.Add(new CompatibleScreen
                    {
                        size = screenElement.GetAttribute("screenSize", AndroidNamespace),
                        density = screenElement.GetAttribute("screenDensity", AndroidNamespace)
                    });
                };
            }

            return screens;
        }


        private void SetCompatibleScreens(HashSet<CompatibleScreen> screens)
        {
            XmlElement element;

            // remove element if argument is null or empty
            if (screens == null || screens.Count == 0)
            {
                element = this.GetNodeByPath("compatible-screens", false) as XmlElement;
                if (element != null)
                {
                    this.GetRootNode().RemoveChild(element);
                }

                return;
            }

            // write new values
            element = this.GetNodeByPath("compatible-screens", true) as XmlElement;
            element.RemoveAll();

            foreach (var screen in screens)
            {
                XmlElement screenElement = Xml.CreateElement("screen");
                screenElement.SetAttribute("screenSize", AndroidNamespace, screen.size);
                screenElement.SetAttribute("screenDensity", AndroidNamespace, screen.density);
                element.AppendChild(screenElement);
            };
        }


        public void AddCompatibleScreens(IEnumerable<CompatibleScreen> compatibleScreens)
        {
            HashSet<CompatibleScreen> screens = GetCompatibleScreens();
            screens.UnionWith(compatibleScreens);
            SetCompatibleScreens(screens);
        }


        public void AddCompatibleScreens(params CompatibleScreen[] compatibleScreens)
        {
            AddCompatibleScreens((IEnumerable<CompatibleScreen>)compatibleScreens);
        }

        #endregion



        #region Application management

        /// <summary>
        /// Gets application element.
        /// </summary>
        /// <returns></returns>
        public XmlElement GetApplicationElement(bool createIfNotExists = true)
        {
            return this.GetNodeByPath("application", createIfNotExists) as XmlElement;
        }


        /// <summary>
        /// Gets a meta-data element with specified name or null if it doesn't exist.
        /// </summary>
        /// <param name="name">The name of element to search.</param>
        /// <returns></returns>
        public XmlElement GetApplicationMetaDataElement(string name)
        {
            return this.FindElement(
                GetApplicationElement(false),
                element => element.LocalName == "meta-data" && this.IsAttributeValueEquals(element, "name", name));
        }


        /// <summary>
        /// Adds new or update existing meta-data element with specified name.
        /// </summary>
        /// <param name="name">A name of the element.</param>
        /// <param name="key">The key to set.</param>
        /// <param name="value">The value to set.</param>
        /// <returns></returns>
        public XmlElement AddApplicationMetaDataElement(string name, string key, string value)
        {
            XmlElement element = GetApplicationMetaDataElement(name);
            if (element == null)
            {
                element = Xml.CreateElement("meta-data");
                element.SetAttribute("name", AndroidNamespace, name);
                element.IsEmpty = true;

                XmlNode node = GetApplicationElement(true);
                node.AppendChild(element);
            }

            if (IsDigitsOnly(value))
            {
                Debug.LogWarning($"Prefer using context.SharedLibrary.GetValuesResource(\"strings.xml\") for digits only value in AndroidManifest meta data element!");
                value = GetUnicodeString(value);
            }
            element.SetAttribute(key, AndroidNamespace, value);
            return element;
            
            
            bool IsDigitsOnly(string checkingValue)
            {
                if (string.IsNullOrEmpty(checkingValue))
                {
                    return false;
                }
                
                const char minDigitChar = '0';
                const char maxDigitChar = '9';
                
                bool result = true;
                foreach (char c in checkingValue)
                {
                    if (c < minDigitChar || c > maxDigitChar)
                    {
                        result = false;
                        break;
                    }
                }

                return result;
            }
            
            
            string GetUnicodeString(string s)
            {
                StringBuilder result = new StringBuilder();
                foreach (char c in s)
                {
                    result.Append(string.Format("\\u{0:x4}", (int)c));
                }
                return result.ToString();
            }
        }


        /// <summary>
        /// Adds new or update existing meta-data element with specified name.
        /// </summary>
        /// <param name="name">A name of the element.</param>
        /// <param name="value">The value to set.</param>
        /// <returns></returns>
        public XmlElement AddApplicationMetaDataElement(string name, string value)
        {
            return AddApplicationMetaDataElement(name, "value", value);
        }


        /// <summary>
        /// Removes a meta-data element with specified name if exists.
        /// </summary>
        /// <param name="name"></param>
        private void RemoveApplicationMetaDataElement(string name)
        {
            XmlElement element = GetApplicationMetaDataElement(name);
            if (element != null)
            {
                GetApplicationElement(false)?.RemoveChild(element);
            }
        }

        #endregion



        #region Activity management
        
        private XmlElement mainActivityElement;
        
        
        public XmlElement MainActivityElement
        {
            get
            {
                if (mainActivityElement == null)
                {
                    XmlNamespaceManager manager = new XmlNamespaceManager(Xml.NameTable);
                    manager.AddNamespace("android", AndroidNamespace);
                    
                    mainActivityElement = Xml.SelectSingleNode(
                        "//activity[intent-filter/action/@android:name=\"android.intent.action.MAIN\"]",
                        manager) as XmlElement;
                }
                
                return mainActivityElement;
            }
        }
        

        /// <summary>
        /// Gets an activity element with specified name or null if it doesn't exist.
        /// </summary>
        /// <param name="name">A name of the activity.</param>
        /// <returns></returns>
        public XmlElement GetActivityElement(string name)
        {
            return this.FindElement(
                GetApplicationElement(false),
                element => element.LocalName == "activity" && this.IsAttributeValueEquals(element, "name", name));
        }


        /// <summary>
        /// Adds new or update existing activity element with specified name.
        /// </summary>
        /// <param name="name">A name of the activity.</param>
        /// <param name="configChanges"></param>
        /// <param name="theme"></param>
        /// <param name="hardwareAccelerated"></param>
        /// <returns></returns>
        public XmlElement AddActivityElement(
            string name, 
            string configChanges = null, 
            string theme = null, 
            bool? hardwareAccelerated = null)
        {
            // <activity android:name="activityName" />
            XmlElement element = GetActivityElement(name);
            if (element == null)
            {
                element = Xml.CreateElement("activity");
                element.SetAttribute("name", AndroidNamespace, name);
                element.IsEmpty = true;

                XmlNode node = GetApplicationElement(true);
                node.AppendChild(element);
            }

            if (!string.IsNullOrEmpty(configChanges))
            {
                element.SetAttribute("configChanges", AndroidNamespace, configChanges);
            }
            if (!string.IsNullOrEmpty(theme))
            {
                element.SetAttribute("theme", AndroidNamespace, theme);
            }
            if (hardwareAccelerated.HasValue)
            {
                element.SetAttribute("hardwareAccelerated", AndroidNamespace, hardwareAccelerated.Value ? "true" : "false");
            }

            return element;
        }

        #endregion



        #region Service management

        /// <summary>
        /// Gets a service element with specified authorities or null if it doesn't exist.
        /// </summary>
        /// <param name="name">A name of the service.</param>
        /// <returns></returns>
        public XmlElement GetServiceElement(string name)
        {
            return this.FindElement(
                GetApplicationElement(false),
                element => element.LocalName == "service" && this.IsAttributeValueEquals(element, "name", name));
        }


        /// <summary>
        /// Adds new or update existing service element with specified authorities.
        /// </summary>
        /// <param name="name">>A name of the service.</param>
        public XmlElement AddServiceElement(string name)
        {
            //<service
            //    android:name="..."
            //</service>

            XmlElement element = GetServiceElement(name);
            if (element == null)
            {
                element = Xml.CreateElement("service");
                element.SetAttribute("name", AndroidNamespace, name);
                element.IsEmpty = true;

                XmlNode node = GetApplicationElement(true);
                node.AppendChild(element);
            }

            return element;
        }

        #endregion
        
        
        
        #region Receiver management

        /// <summary>
        /// Gets a receiver element with specified name or null if it doesn't exist.
        /// </summary>
        /// <param name="name">A name of the receiver.</param>
        /// <returns></returns>
        public XmlElement GetReceiverElement(string name)
        {
            return this.FindElement(
                GetApplicationElement(false),
                element => element.LocalName == "receiver" && this.IsAttributeValueEquals(element, "name", name));
        }


        /// <summary>
        /// Adds new or update existing receiver element with specified name.
        /// </summary>
        /// <param name="name">A name of the receiver.</param>
        public XmlElement AddReceiverElement(string name)
        {
            XmlElement element = GetReceiverElement(name);
            if (element == null)
            {
                element = Xml.CreateElement("receiver");
                element.SetAttribute("name", AndroidNamespace, name);
                element.IsEmpty = true;

                XmlNode node = GetApplicationElement(true);
                node.AppendChild(element);
            }

            return element;
        }

        #endregion



        #region Provider management

        /// <summary>
        /// Gets a provider element with specified authorities or null if it doesn't exist
        /// </summary>
        /// <param name="name"></param>
        /// <param name="authorities"></param>
        /// <returns></returns>
        public XmlElement GetProviderElement(string name, string authorities)
        {
            return this.FindElement(
                GetApplicationElement(false),
                element => element.LocalName == "provider" &&
                    this.IsAttributeValueEquals(element, "name", name) &&
                    this.IsAttributeValueEquals(element, "authorities", authorities));
        }


        /// <summary>
        /// Adds new or update existing provider element with specified authorities.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="authorities"></param>
        /// <param name="exported"></param>
        public XmlElement AddProviderElement(string name, string authorities, bool exported = false)
        {
            //<provider
            //    android:name="android.support.v4.content.FileProvider"
            //    android:authorities="org.hive.foundation.sharedialogfileprovider"
            //    android:exported="false">
            //</provider>

            XmlElement element = GetProviderElement(name, authorities);
            if (element == null)
            {
                element = Xml.CreateElement("provider");
                element.SetAttribute("name", AndroidNamespace, name);
                element.SetAttribute("authorities", AndroidNamespace, authorities);
                element.IsEmpty = true;

                XmlNode node = GetApplicationElement(true);
                node.AppendChild(element);
            }

            element.SetAttribute("exported", AndroidNamespace, exported ? "true" : "false");
            return element;
        }

        #endregion
        


        #region File provider management

        /// <summary>
        /// Gets a file provider element with specified authorities or null if it doesn't exist
        /// </summary>
        /// <param name="authorities"></param>
        /// <returns></returns>
        public XmlElement GetFileProviderElement(string authorities)
        {
            return this.FindElement(
                GetApplicationElement(false),
                element => element.LocalName == "provider" && this.IsAttributeValueEquals(element, "authorities", authorities));
        }


        /// <summary>
        /// Adds new or update existing file provider element with specified authorities.
        /// </summary>
        /// <param name="authorities"></param>
        /// <param name="resource"></param>
        /// <param name="grantUriPermissions"></param>
        /// <param name="exported"></param>
        /// <param name="name"></param>
        public XmlElement AddFileProviderElement(string authorities, string resource, bool grantUriPermissions = true, bool exported = false, string name = null)
        {
            //<provider
            //    android:name="android.support.v4.content.FileProvider"
            //    android:authorities="org.hive.foundation.sharedialogfileprovider"
            //    android:grantUriPermissions="true"
            //    android:exported="false">
            //    <meta-data
            //        android:name="android.support.FILE_PROVIDER_PATHS"
            //        android:resource="@xml/shared_paths" />
            //</provider>

            XmlElement element = GetFileProviderElement(authorities);
            if (element == null)
            {
                if (string.IsNullOrEmpty(name))
                {
                    name = "android.support.v4.content.FileProvider";
                }
                
                element = Xml.CreateElement("provider");
                element.SetAttribute("name", AndroidNamespace, name);
                element.SetAttribute("authorities", AndroidNamespace, authorities);
                element.IsEmpty = true;

                XmlNode node = GetApplicationElement(true);
                node.AppendChild(element);
            }

            element.SetAttribute("grantUriPermissions", AndroidNamespace, grantUriPermissions ? "true" : "false");
            element.SetAttribute("exported", AndroidNamespace, exported ? "true" : "false");

            // meta-data
            XmlElement md = this.FindElement(element, e => e.LocalName == "meta-data" && this.IsAttributeValueEquals(e, "name", "android.support.FILE_PROVIDER_PATHS"));
            if (md == null)
            {
                md = Xml.CreateElement("meta-data");
                md.SetAttribute("name", AndroidNamespace, "android.support.FILE_PROVIDER_PATHS");
                md.IsEmpty = true;
                element.AppendChild(md);
            }

            md.SetAttribute("resource", AndroidNamespace, resource);

            return element;
        }

        #endregion
        
        
        
        #region Package management
        
        /// <summary>
        /// Gets queries element.
        /// </summary>
        /// <returns></returns>
        public XmlElement GetQueriesElement(bool createIfNotExists = true)
        {
            return this.GetNodeByPath("queries", createIfNotExists) as XmlElement;
        }

        /// <summary>
        /// Gets a package element with specified authorities or null if it doesn't exist.
        /// </summary>
        /// <param name="name">A name of the package.</param>
        /// <returns></returns>
        public XmlElement GetPackageElement(string name)
        {
            return this.FindElement(
                GetQueriesElement(false),
                element => element.LocalName == "package" && this.IsAttributeValueEquals(element, "name", name));
        }


        /// <summary>
        /// Adds new or update existing package element with specified authorities.
        /// </summary>
        /// <param name="name">>A name of the package.</param>
        public XmlElement AddPackageElement(string name)
        {
            //<queries>
            //<package
            //    android:name="..."
            //</package>
            //...
            //</queries>

            XmlElement element = GetPackageElement(name);
            if (element == null)
            {
                element = Xml.CreateElement("package");
                element.SetAttribute("name", AndroidNamespace, name);
                element.IsEmpty = true;

                XmlNode node = GetQueriesElement(true);
                node.AppendChild(element);
            }

            return element;
        }

        #endregion



        #region Manifest merge tools

        // documentation: https://developer.android.com/studio/build/manifest-merge.html

        /// <summary>
        /// Allows to replace the lower-priority element completely. 
        /// That is, if there is a matching element in the lower-priority manifest, ignore it and use this element exactly as it appears in this manifest. 
        /// </summary>
        /// <param name="element">The target element.</param>
        public void AddElementToToolsRemove(XmlElement element)
            => AddElementOperationToTools(element, "remove");


        /// <summary>
        /// Allows to replace the specified attributes in the lower-priority manifest with those from this manifest.
        /// In other words, always keep the higher-priority manifest's values.
        /// </summary>
        /// <param name="element">The target element.</param>
        /// <param name="attributes">The attributes to replace.</param>
        public void AddAttributesToToolsReplace(XmlElement element, params string[] attributes)
            => AddAttributesOperationToTools(element, "replace", attributes);


        /// <summary>
        /// Allows to remove the specified attributes from the merged manifest.
        /// Although it seems like you could instead just delete these attributes, it's necessary to use this when the lower-priority manifest file
        /// does include these attributes and you want to ensure they do not go into the merged manifest.
        /// </summary>
        /// <param name="element">The target element.</param>
        /// <param name="attributes">The attributes to remove.</param>
        public void AddAttributesToToolsRemove(XmlElement element, params string[] attributes)
            => AddAttributesOperationToTools(element, "remove", attributes);


        private void AddAttributesOperationToTools(XmlElement element, string operation, params string[] attributes)
        {
            string value = element.GetAttribute(operation, AndroidToolsNamespace);
            HashSet<string> newTags = string.IsNullOrEmpty(value) ?
                new HashSet<string>() :
                new HashSet<string>(value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));

            newTags.UnionWith(attributes);

            element.SetAttribute(operation, AndroidToolsNamespace, string.Join(",", newTags));
        }


        private void AddElementOperationToTools(XmlElement element, string operation)
        {
            element.SetAttribute("node", AndroidToolsNamespace, operation);
        }

        #endregion



        #region Other properties and methods

        /// <summary>
        /// Gets or sets a min Android SDK version.
        /// </summary>
        public int MinSdkVersion
        {
            get
            {
                XmlElement element = this.GetNodeByPath("uses-sdk") as XmlElement;
                if (element == null)
                {
                    return 0;
                }

                string rs = element.GetAttribute("minSdkVersion", AndroidNamespace);
                if (string.IsNullOrEmpty(rs))
                {
                    return 0;
                }

                return int.Parse(rs);
            }
            set
            {
                XmlElement element = this.GetNodeByPath("uses-sdk", true) as XmlElement;
                element.SetAttribute("minSdkVersion", AndroidNamespace, value.ToString());
            }
        }


        /// <summary>
        /// Gets or sets whether the package is marked as debuggable.
        /// </summary>
        public bool Debuggable
        {
            get
            {
                XmlElement element = GetApplicationElement();
                string rs = element.GetAttribute("debuggable", AndroidNamespace);

                return !string.IsNullOrEmpty(rs) && rs.ToLower() == "true";
            }
            set
            {
                XmlElement element = GetApplicationElement();
                element.SetAttribute("debuggable", AndroidNamespace, value ? "true" : "false");
            }
        }


        /// <summary>
        /// Gets or sets whether the application supports split screen.
        /// </summary>
        public bool ResizeableActivity
        {
            get
            {
                XmlElement element = GetApplicationElement();
                string rs = element.GetAttribute("resizeableActivity", AndroidNamespace);

                // The attribute value defaults to true.
                // see https://developer.android.com/guide/topics/ui/multi-window.html
                return string.IsNullOrEmpty(rs) || rs.ToLower() == "true";
            }

            set
            {
                XmlElement element = GetApplicationElement();
                element.SetAttribute("resizeableActivity", AndroidNamespace, value ? "true" : "false");
            }
        }


        /// <summary>
        /// Gets or sets maximum aspect ratio of screen.
        /// <para>
        /// Value 2.4 (12:5) is recommended by google https://developer.android.com/guide/practices/screens-distribution"
        /// </para>
        /// </summary>
        public float? MaxAspectRatio
        {
            get
            {
                XmlElement element = GetApplicationMetaDataElement("android.max_aspect");
                if (element == null)
                {
                    return null;
                }

                string rs = element.GetAttribute("value", AndroidNamespace);
                return float.Parse(rs, NumberStyles.Float, CultureInfo.InvariantCulture);
            }
            set
            {
                if (value == null)
                {
                    RemoveApplicationMetaDataElement("android.max_aspect");
                    return;
                }

                XmlElement element = GetApplicationMetaDataElement("android.max_aspect") ??
                    AddApplicationMetaDataElement("android.max_aspect", "");

                element.SetAttribute("value", AndroidNamespace, value.Value.ToString(CultureInfo.InvariantCulture));
            }
        }

        /// <summary>
        /// Gets unique package name
        /// </summary>
        /// <returns></returns>
        public static string GeneratePackageName()
        {
            var uid = Regex.Replace(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), "[/+=]", "");
            return $"{PackageRootName}.undefined_{uid}";
        }


        /// <summary>
        /// Saves manifest.
        /// </summary>
        public void Save()
        {
            Xml.Save(OutputPath);
        }

        #endregion
    }
}
