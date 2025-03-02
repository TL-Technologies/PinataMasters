using Modules.Hive.Editor.BuildUtilities.Android;


namespace Modules.Hive.Editor.BuildUtilities
{
    public interface IAndroidBuildPreprocessorContext : IBuildPreprocessorContext
    {
        /// <summary>
        /// Gets a gradle build script of the application.
        /// </summary>
        GradleScript GradleScript { get; }

        /// <summary>
        /// Gets an Android manifest of the application.
        /// </summary>
        AndroidManifest AndroidManifest { get; }
        
        /// <summary>
        /// Gets Launcher manifest of the application.
        /// </summary>
        AndroidManifest LauncherManifest { get; }

        /// <summary>
        /// Gets a shared library to store any resources of the project.
        /// </summary>
        AndroidLibrary SharedLibrary { get; }

        /// <summary>
        /// Creates a new temporary android library at Assets/Plugins/Android.
        /// </summary>
        /// <param name="name">A name of the new android library.</param>
        /// <param name="package">A package name of the new android library.</param>
        AndroidLibrary CreateTemporaryAndroidLibrary(string name, string package = null);

        /// <summary>
        /// Gets or sets whether the android backup system is allowed for the project.
        /// </summary>
        bool IsAllowBackup { get; set; }

        /// <summary>
        /// Gets a config with android backup rules if it exists; otherwise, null.
        /// </summary>
        BackupRulesResource BackupRulesIfExists { get; }

        /// <summary>
        /// Gets a config with android backup rules.
        /// The config will be created if it doesn't exist.
        /// </summary>
        BackupRulesResource BackupRules { get; }

        /// <summary>
        /// Gets a network security config if it exists; otherwise, null.
        /// </summary>
        NetworkSecurityConfigResource NetworkSecurityConfigIfExist { get; }

        /// <summary>
        /// Gets a network security config. It will be created if doesn't exist.
        /// </summary>
        NetworkSecurityConfigResource NetworkSecurityConfig { get; }

        /// <summary>
        /// Gets or sets a package sign options.
        /// </summary>
        PackageSignOptions PackageSignOptions { get; set; }
    }
}
