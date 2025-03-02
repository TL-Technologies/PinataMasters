using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Modules.Hive;
using Modules.Hive.Editor;
using Modules.Hive.Reflection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;


namespace Modules.General.Editor.Tests
{
    internal static class PluginsVersionsUtilities
    {
        private const string PackagesNamePrefix = "com.playgendary";
        private const string PackageJsonFileName = "package.json";
        private const string SubmodulesAssetPathRegex = @"^\s*path ?= ?(.*)\s*$";
        private const string SpreadsheetId = "1YVAD-BUnJhOmeihCfTOuQ9WT9mlwcIAAqMLwGI6BGow";
        private const string WorksheetName = "'Versions'";
        private static readonly string CredentialsJsonPath = UnityPath.Combine(
            GeneralPluginHierarchy.Instance.RootPath,
            "Tests",
            "Editor",
            "PluginsVersions",
            "plugins_versions_sheet_credentials.json");
        
        private static readonly string GitModulesFilePath = UnityPath.Combine(UnityPath.ProjectPath, ".gitmodules");
        private static readonly Dictionary<string, string> DefaultPluginNames = new Dictionary<string, string>()
        {
            { "AB Test", "AbTest" },
            { "Appsflyer", "AppsFlyer" },
            { "Legacy Vibration", "LegacyVibration" },
            { "MoPub Advertising", "MoPub" },
            { "Ironsource", "IronSource" }
        };
        
        
        public static Dictionary<string, string> GetMinimumPluginsVersions(BuildTargetGroup buildTargetGroup)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            if (buildTargetGroup != BuildTargetGroup.iOS && buildTargetGroup != BuildTargetGroup.Android)
            {
                return result;
            }
            
            string platformName = buildTargetGroup == BuildTargetGroup.Android ? "Android" : "iOS";
            
            GoogleCredential credential = GoogleCredential
                .FromFile(CredentialsJsonPath)
                .CreateScoped(SheetsService.Scope.SpreadsheetsReadonly);
            SheetsService sheetsService = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential
            });
            
            SpreadsheetsResource.ValuesResource.GetRequest request = sheetsService.Spreadsheets.Values.Get(
                SpreadsheetId,
                WorksheetName);
            request.MajorDimension = SpreadsheetsResource.ValuesResource.GetRequest.MajorDimensionEnum.COLUMNS;
            
            IList<IList<object>> values = request.Execute().Values;
            IList<object> platformVersions = null;
            
            foreach (IList<object> list in values)
            {
                if (list != null && list.Count > 0 && list[0].Equals(platformName))
                {
                    platformVersions = list;
                    break;
                }
            }
            
            if (platformVersions != null)
            {
                IList<object> pluginNames = values[0];
                int platformVersionsCount = platformVersions.Count;
                // Skip first elements because they are service fields
                for (int i = 1; i < pluginNames.Count; i++)
                {
                    string pluginVersion = (i < platformVersionsCount) ? (string)platformVersions[i] : "0.0.0";
                    string pluginName = GetStandardizedPluginName((string)pluginNames[i]);
                    result.Add(pluginName, pluginVersion);
                }
            }
            
            return result;
        }
        
        
        public static Dictionary<string, string> GetCurrentPluginsVersions()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            
            // Find submodules
            if (File.Exists(GitModulesFilePath))
            {
                string gitModulesFileContent;
                using (StreamReader streamReader = File.OpenText(GitModulesFilePath))
                {
                    gitModulesFileContent = streamReader.ReadToEnd();
                }
                
                MatchCollection matches = Regex.Matches(
                    gitModulesFileContent,
                    SubmodulesAssetPathRegex,
                    RegexOptions.Multiline);
                foreach (Match match in matches)
                {
                    if (match.Success)
                    {
                        AddPluginInfoToDictionary(
                            result,
                            UnityPath.Combine(UnityPath.ProjectPath, match.Groups[1].Value, PackageJsonFileName));
                    }
                }
            }
            
            // Find packages
            Func<PackageInfo[]> packageInfoDelegate = ReflectionHelper.CreateDelegateToMethod<Func<PackageInfo[]>>(
                typeof(PackageInfo),
                "GetAll",
                BindingFlags.NonPublic | BindingFlags.Static,
                true);
            PackageInfo[] packageInfos = packageInfoDelegate();
            foreach (PackageInfo packageInfo in packageInfos)
            {
                AddPluginInfoToDictionary(
                    result,
                    UnityPath.Combine(packageInfo.resolvedPath, PackageJsonFileName));
            }

            return result;
            
            
            void AddPluginInfoToDictionary(IDictionary<string, string> dictionary, string packageJsonPath)
            {
                UnityPackageInfo packageInfo = UnityPackageInfo.Open(packageJsonPath);
                if (packageInfo != null && packageInfo.name.StartsWith(PackagesNamePrefix))
                {
                    string pluginName = GetStandardizedPluginName(packageInfo.displayName);
                    string pluginVersion = packageInfo.version;
                            
                    dictionary.Add(pluginName, pluginVersion);
                }
            }
        }
        
        
        internal static bool IsVersionsRequirementsSatisfied(
            IReadOnlyDictionary<string, string> currentVersions, 
            IReadOnlyDictionary<string, string> requiredVersions,
            out string comparisonReport)
        {
            StringBuilder failedComparisons = new StringBuilder();
            StringBuilder successfulComparisons = new StringBuilder();
            bool isComparisonSuccessful = true;

            foreach (string pluginName in currentVersions.Keys)
            {
                string currentVersion = currentVersions[pluginName];
                if (requiredVersions.ContainsKey(pluginName))
                {
                    string requiredVersion = requiredVersions[pluginName];
                    if (GetPluginVersionFromString(currentVersion) >= GetPluginVersionFromString(requiredVersion))
                    {
                        successfulComparisons.Append($"{pluginName}: {currentVersion} >= {requiredVersion}\n");
                    }
                    else
                    {
                        isComparisonSuccessful = false;
                        failedComparisons.Append($"{pluginName}: {currentVersion} < {requiredVersion}\n");
                    }
                }
                else
                {
                    successfulComparisons.Append($"{pluginName}: {currentVersion} >= 0.0.0\n");
                }
            }
            
            comparisonReport = isComparisonSuccessful ? 
                successfulComparisons.ToString() : 
                $"Failed:\n{failedComparisons}\nSuccessful:\n{successfulComparisons}";
            return isComparisonSuccessful;
            
            
            ExtendedVersion GetPluginVersionFromString(string version)
            {
                // Remove part after first hyphen (-preview.1 etc.)
                const char stopCharacter = '-';
                int charLocation = version.IndexOf(stopCharacter);
                if (charLocation > 0)
                {
                    version = version.Substring(0, charLocation);
                }
                
                return new ExtendedVersion(version);
            }
        }
        

        private static string GetStandardizedPluginName(string pluginName)
        {
            return DefaultPluginNames.ContainsKey(pluginName) ? DefaultPluginNames[pluginName] : pluginName;
        }
    }
}
