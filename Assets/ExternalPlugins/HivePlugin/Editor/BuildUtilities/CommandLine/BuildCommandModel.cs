using Modules.Hive.Editor.BuildUtilities.Android;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;


namespace Modules.Hive.Editor.BuildUtilities
{
    /// <summary>
    /// Required arguments:
    ///     -buildTarget {ios | android}
    ///     
    /// Optional arguments:
    ///     --androidTarget { GooglePlay | Amazon }
    ///     --appBundle
    ///     --buildCompression "Default | LZ4 | LZ4HC"
    ///     --buildNumber "number"
    ///     --ciBuild
    ///     --configuration {debug | release}
    ///     --createSymbolsFile
    ///     --define A [B] [...]
    ///     --exportAndroidProject "path"
    ///     --keyStorePath "path"
    ///     --location "path"
    /// </summary>
    public class BuildCommandModel : CommandModel
    {
        #region Fields

        private AndroidTarget androidTarget = AndroidTarget.None;

        #endregion
        
        
        
        #region Properties
        
        public bool IsDebugEnabled { get; private set; }


        public bool IsEnableDeepProfiling { get; private set; }


        public bool IsWaitForManagedDebugger { get; private set; }


        public BuildTarget BuildTarget { get; private set; } = BuildTarget.NoTarget;


        public BuildOptions BuildCompressionOption { get; private set; } = BuildOptions.None;


        public string BuildLocation { get; private set; }

        public string PipelineName { get; private set; }

        public string KeyStorePath { get; private set; }
        
        
        public Dictionary<BuildSetting, string> BuildSettings { get; private set; }


        public PreprocessorDirectivesCollection PreprocessorDirectives { get; }
        
        #endregion


        
        #region Class lifecycle
        
        public BuildCommandModel(string[] args)
        {
            PreprocessorDirectives = new PreprocessorDirectivesCollection();
            BuildSettings = new Dictionary<BuildSetting, string>(new BuildSettingComparer());

            RegisterOptionParser("androidTarget", OnParseAndroidTarget);
            RegisterOptionParser("buildCompression", OnParseBuildCompression);
            RegisterOptionParser("buildTarget", OnParseBuildTarget); // Unity option
            RegisterOptionParser("configuration", OnParseConfiguration);
            RegisterOptionParser("define", OnParseDefines);
            RegisterOptionParser("enableDeepProfilingSupport", OnEnableDeepProfilingSupport);
            RegisterOptionParser("waitForManagedDebugger", OnWaitForManagedDebugger);
            RegisterOptionParser("exportAndroidProject", OnParseExportAndroidProjectPath);
            RegisterOptionParser("keyStorePath", OnParseKeyStorePath);
            RegisterOptionParser("location", OnParseLocation);
            RegisterOptionParser("pipelineName", OnParsePipelineName);
            RegisterOptionParser(BuildSetting.AppBundleNecessity.Name, OnParseAppBundle);
            RegisterOptionParser(BuildSetting.BuildNumber.Name, OnParseBuildNumber);
            RegisterOptionParser(BuildSetting.CiBuildFlag.Name, OnParseCiBuild);
            RegisterOptionParser(BuildSetting.SymbolsFileNecessity.Name, OnParseSymbolsFileNecessity);
            RegisterOptionParser(BuildSetting.EnableCrashReportAPI.Name, OnParseEnableCrashReportAPI);
            Parse(args);
        }
        
        #endregion


        public BuildPlayerOptions CreateBuildPlayerOptions()
        {
            BuildPlayerOptions buildOptions = BuildPipelineFactory.GetBuildPlayerOptionsFromEditor(BuildTarget);
        
            // Configure build location path
            if (string.IsNullOrEmpty(BuildLocation))
            {
                buildOptions.locationPathName = UnityPath.Combine("Builds", FileSystemUtilities.GetValidFileName(PlayerSettings.productName));
            }
            else if (!Path.IsPathRooted(BuildLocation) && !BuildLocation.StartsWith("Builds"))
            {
                buildOptions.locationPathName = UnityPath.Combine("Builds", BuildLocation);
            }
            else
            {
                buildOptions.locationPathName = BuildLocation;
            }
        
            // Configure debug options
            if (IsDebugEnabled)
            {
                buildOptions.options |= BuildOptions.Development | BuildOptions.AllowDebugging | BuildOptions.ConnectWithProfiler;

                if (IsEnableDeepProfiling)
                {
                    buildOptions.options |= BuildOptions.EnableDeepProfilingSupport;
                }

                if (IsWaitForManagedDebugger)
                {
                    EditorUserBuildSettings.waitForManagedDebugger = true;
                }
            }

            // Configure compression option. BuildOptions.None means default compression method
            if (BuildCompressionOption != BuildOptions.None)
            {
                buildOptions.options |= BuildCompressionOption;
            }
                
            return buildOptions;
        }
        
        
        public BuildPipelineContext CreateDefaultPipelineContext(BuildPlayerOptions? buildOptions = null)
        {
            var context = BuildPipelineFactory.CreateDefaultPipelineContext(
                buildOptions ?? CreateBuildPlayerOptions(),
                androidTarget);
            context.PreprocessorDirectives.UnionWith(PreprocessorDirectives);
            context.BuildSettings = BuildSettings;
            
            if (context is AndroidBuildPipelineContext androidContext)
            {
                if (!string.IsNullOrWhiteSpace(KeyStorePath))
                {
                    androidContext.PackageSignOptions = new PackageSignOptions(KeyStorePath);
                }
            }
        
            return context;
        }


        #region Parsing

        private void OnParseConfiguration(CliToken token, CliReader reader)
        {
            CliToken value = reader.GetNextValue();
            IsDebugEnabled = CliToken.Equals(value, "debug");
        }


        private void OnParseBuildTarget(CliToken token, CliReader reader)
        {
            CliToken value = reader.GetNextValue();
            if (value == null)
            {
                throw new Exception("No value specified for argument -buildTarget");
            }

            string target = value.text.ToLowerInvariant();
            switch (target)
            {
                case "android":
                    BuildTarget = BuildTarget.Android;
                    break;
                case "ios": 
                    BuildTarget = BuildTarget.iOS;
                    break;
                case "osxuniversal":
                    BuildTarget = BuildTarget.StandaloneOSX;
                    break;
                case "webgl":
                    BuildTarget = BuildTarget.WebGL;
                    break;
                case "win64":
                    BuildTarget = BuildTarget.StandaloneWindows64;
                    break;
                default:
                    throw new Exception($"Invalid value of argument -buildTarget. {target} is not supported.");
            }
        }

        
        private void OnParseBuildCompression(CliToken token, CliReader reader)
        {
            CliToken value = reader.GetNextValue();
            if (value == null)
            {
                return;
            }
            
            string compressionStringValue = value.text.ToLowerInvariant();
            switch (compressionStringValue)
            {
                case "lz4":
                    BuildCompressionOption = BuildOptions.CompressWithLz4;
                    break;
                case "lz4hc":
                    BuildCompressionOption = BuildOptions.CompressWithLz4HC;
                    break;
            }
        }

        
        private void OnParseLocation(CliToken token, CliReader reader)
        {
            CliToken value = reader.GetNextValue();
            if (value != null)
            {
                BuildLocation = value.text;
            }
        }
        
        
        private void OnParsePipelineName(CliToken token, CliReader reader)
        {
            CliToken value = reader.GetNextValue();
            if (value != null)
            {
                PipelineName = value.text;
            }
        }

        
        private void OnParseDefines(CliToken token, CliReader reader)
        {
            while (true)
            {
                CliToken value = reader.GetNextValue();
                if (value == null)
                {
                    break;
                }
                PreprocessorDirectives.UnionWith(value.text);
            }
        }


        private void OnParseKeyStorePath(CliToken token, CliReader reader)
        {
            CliToken value = reader.GetNextValue();
            if (value != null)
            {
                KeyStorePath = value.text;
            }
        }
        
        
        private void OnParseAppBundle(CliToken token, CliReader reader)
        {
            // Assume, that key existence equivalent to AppBundle necessity, so we don't care about value part
            BuildSettings[BuildSetting.AppBundleNecessity] = string.Empty;
        }
        
        
        private void OnParseCiBuild(CliToken token, CliReader reader)
        {
            // Assume, that key existence equivalent to CiBuild == true, so we don't care about value part
            BuildSettings[BuildSetting.CiBuildFlag] = string.Empty;
        }
        
        
        private void OnParseBuildNumber(CliToken token, CliReader reader)
        {
            CliToken value = reader.GetNextValue();
            if (value != null)
            {
                BuildSettings[BuildSetting.BuildNumber] = value.text;
            }
        }
        
        
        private void OnParseSymbolsFileNecessity(CliToken token, CliReader reader)
        {
            // Assume, that key existence equivalent to symbols file necessity, so we don't care about value part
            BuildSettings[BuildSetting.SymbolsFileNecessity] = string.Empty;
        }


        private void OnEnableDeepProfilingSupport(CliToken token, CliReader reader)
        {
            CliToken value = reader.GetNextValue();
            IsEnableDeepProfiling = value.text.Equals("true");
        }


        private void OnParseEnableCrashReportAPI(CliToken token, CliReader reader)
        {
            CliToken value = reader.GetNextValue();
            if (value != null)
            {
                BuildSettings[BuildSetting.EnableCrashReportAPI] = value.text;
            }
        }


        private void OnParseExportAndroidProjectPath(CliToken token, CliReader reader)
        {
            CliToken value = reader.GetNextValue();
            if (value != null)
            {
                BuildSettings[BuildSetting.ExportAndroidProjectPath] = value.text;
            }
        }
        
        
        private void OnParseAndroidTarget(CliToken token, CliReader reader)
        {
            CliToken value = reader.GetNextValue();
            if (value == null)
            {
                return;
            }
            
            string androidTargetString = value.text.ToLowerInvariant();
            switch (androidTargetString)
            {
                case "huawei":
                    androidTarget = AndroidTarget.Huawei;
                    break;
                case "amazon":
                    androidTarget = AndroidTarget.Amazon;
                    break;
                case "googleplay":
                    androidTarget = AndroidTarget.GooglePlay;
                    break;
                default:
                    androidTarget = AndroidTarget.GooglePlay;
                    break;
            }
        }


        private void OnWaitForManagedDebugger(CliToken token, CliReader reader)
        {
            CliToken value = reader.GetNextValue();
            IsWaitForManagedDebugger = value.text.Equals("true");
        }


        protected override void OnValidate()
        {
            if (BuildTarget == BuildTarget.NoTarget)
            {
                throw new Exception("Missing argument -buildTarget.");
            }
        }

        #endregion
    }
}
