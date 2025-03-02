using Modules.Hive.Editor.BuildUtilities;
using Modules.Hive.Editor.BuildUtilities.Ios;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;


namespace Modules.General.Editor.BuildProcess
{
    internal class ApplePlatformBuildProcess : IBuildPostprocessor<IIosBuildPostprocessorContext>
    {
        private static readonly List<string> ForbiddenUrlSchemes = new List<string>() 
        { 
            "javascript", "vbscript", "data", "blob", "http", "https", 
            "mailto", "livescript", "facetime", "facetime-audio"
        };
        
        
        public void OnPostprocessBuild(IIosBuildPostprocessorContext context)
        {
            context.InfoPlist.AddUrlSchemes(new[] { VerifyUrlScheme(Application.identifier) });
                
            // Add system frameworks
            context.PbxProject.AddSystemFramework(Framework.AdSupport, true);
            context.PbxProject.AddSystemFramework(Framework.GameKit, true);
            context.PbxProject.AddSystemFramework(Framework.MetalKit);
            context.PbxProject.AddSystemFramework(Framework.MobileCoreServices, true);
            context.PbxProject.AddSystemFramework(Framework.Security, true);
            context.PbxProject.AddSystemFramework(Framework.StoreKit);

            // Add system dynamic libs
            context.PbxProject.AddSystemDylib(DyLib.LibZ);
            context.PbxProject.AddSystemDylib(DyLib.LibXml2);
            context.PbxProject.AddSystemDylib(DyLib.LibSqlite3, true);

            // Add linker flags
            context.PbxProject.AddLinkerFlags(PbxProjectTargetType.Default, LinkerFlag.ObjC);
            
            // Delete unnecessary loaders for decreasing build size
            DeleteUnnecessaryLoaders(context.BuildPath);
            
            
            // Checks according http://bit.ly/2JNVd2e
            string VerifyUrlScheme(string urlScheme)
            {
                if (ForbiddenUrlSchemes.Contains(urlScheme))
                {
                    Debug.LogWarning($"You can't use url scheme {urlScheme}!");
                    return string.Empty;
                }
            
                // Remove not allowed chars
                string result = Regex.Replace(urlScheme, @"[^a-zA-Z0-9\+\-\.]", string.Empty);
                // Remove leading numbers 
                result = Regex.Replace(result, @"^\d+", string.Empty);
            
                if (string.IsNullOrEmpty(result))
                {
                    Debug.LogWarning($"Url scheme {urlScheme} is not valid!");
                }
            
                return result;
            }
        }
        
        
        private static void DeleteUnnecessaryLoaders(string path)
        {
            // This file doesn't exist, if we use storyboard instead of png images for loader
            string jsonPath = path + "/Unity-iPhone/Images.xcassets/LaunchImage.launchimage/Contents.json";
            if (File.Exists(jsonPath))
            {
                // Delete unnecessary loaders
                string screenStartOrientation = "portrait";
                List<string> images = new List<string>();
    
                int assetPositionInPath = Application.dataPath.IndexOf("Assets");
                string projectSettingsPath = Application.dataPath.Substring(0, assetPositionInPath) +
                    "ProjectSettings/ProjectSettings.asset";
    
                bool isLandscapeOrientation = false;
                bool isPortraitOrientation = false;
    
                StreamReader projectSettingsReader = new StreamReader(projectSettingsPath);
                string allText = projectSettingsReader.ReadToEnd();
                projectSettingsReader.Close();
                isLandscapeOrientation = allText.IndexOf("allowedAutorotateToLandscapeRight: 1") != -1 ||
                    allText.IndexOf("allowedAutorotateToLandscapeLeft: 1") != -1;
                isPortraitOrientation = allText.IndexOf("allowedAutorotateToPortrait: 1") != -1 ||
                    allText.IndexOf("allowedAutorotateToPortraitUpsideDown: 1") != -1;
    
                if (!isLandscapeOrientation &&
                    (PlayerSettings.defaultInterfaceOrientation == UIOrientation.LandscapeLeft ||
                        PlayerSettings.defaultInterfaceOrientation == UIOrientation.LandscapeRight))
                {
                    isLandscapeOrientation = true;
                }
    
                if (!isPortraitOrientation &&
                    (PlayerSettings.defaultInterfaceOrientation == UIOrientation.Portrait ||
                        PlayerSettings.defaultInterfaceOrientation ==
                        UIOrientation.PortraitUpsideDown))
                {
                    isPortraitOrientation = true;
                }
    
                if (!isPortraitOrientation)
                {
                    images.Add("Default.png");
                    images.Add("Default-Portrait.png");
                    images.Add("Default-Portrait@2x.png");
                    images.Add("Default-Portrait@3x.png");
                }
    
                if (!isLandscapeOrientation)
                {
                    images.Add("Default.png");
                    images.Add("Default-Landscape.png");
                    images.Add("Default-Landscape@2x.png");
                    images.Add("Default-Landscape@3x.png");
                }
                else
                {
                    screenStartOrientation = "landscape";
                }
    
                foreach (string image in images)
                {
                    string imagePath = path + "/Unity-iPhone/Images.xcassets/LaunchImage.launchimage/" + image;
    
                    if (File.Exists(imagePath))
                    {
                        CustomDebug.Log("Remove : " + image);
                        File.Delete(imagePath);
                    }
                }
    
                // Fix loaders JSON
                FileInfo projectFileInfo = new FileInfo(jsonPath);
                StreamReader reader = projectFileInfo.OpenText();
                string contents = reader.ReadToEnd();
                reader.Close();
    
                Dictionary<object, object> dict = MiniJSON.Json.Deserialize<Dictionary<object, object>>(contents);
                object[] imageArray = (object[]) dict["images"];
                List<object> imageList = new List<object>(imageArray);
    
                // Remove all entries that have deleted loader images
                foreach (Dictionary<string, object> imageInfo in imageArray)
                {
                    foreach (string image in images)
                    {
                        string imageName = GetStringValueFromInfoByKey(imageInfo, "filename");
                        string systemVersion = GetStringValueFromInfoByKey(imageInfo,"minimum-system-version");
                        string orientation = GetStringValueFromInfoByKey(imageInfo, "orientation");
                        string idiom = GetStringValueFromInfoByKey(imageInfo, "idiom");
    
                        if ((imageName.Equals(image) || systemVersion.Equals(string.Empty)) ||
                            (!screenStartOrientation.Equals(orientation) && idiom.Equals("ipad")))
                        {
                            imageList.Remove(imageInfo);
                            break;
                        }
                    }
                }
    
                dict["images"] = imageList;
    
                string newContent = MiniJSON.Json.Serialize(dict);
                StreamWriter writer = new StreamWriter(jsonPath);
                writer.Write(newContent);
                writer.Close();
            }
                
                
            string GetStringValueFromInfoByKey(Dictionary<string, object> info, string infoKey)
            {
                object curObj;
        
                if (!info.TryGetValue(infoKey, out curObj))
                {
                    curObj = string.Empty;
                }

                return curObj as string;
            }
        }
    }
}
