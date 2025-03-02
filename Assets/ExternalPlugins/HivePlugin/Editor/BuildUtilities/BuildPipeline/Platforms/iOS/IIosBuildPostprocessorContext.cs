using Modules.Hive.Editor.BuildUtilities.Ios;


namespace Modules.Hive.Editor.BuildUtilities
{
    public interface IIosBuildPostprocessorContext : IBuildPostprocessorContext
    {
        /// <summary>
        /// Gets main pbx file of the project.
        /// </summary>
        PbxProject PbxProject { get; }

        /// <summary>
        /// Gets an Info.plist of the project.
        /// </summary>
        InfoPlist InfoPlist { get; }

        /// <summary>
        /// Gets entitlements of the project.
        /// </summary>
        EntitlementsPlist Entitlements { get; }

        /// <summary>
        /// Returns a path for placing native files of the module inside xcode project.
        /// </summary>
        /// <param name="moduleName">A name of the module.</param>
        /// <returns></returns>
        string GetDestinationPath(string moduleName);
    }
}
