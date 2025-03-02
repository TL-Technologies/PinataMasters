namespace Modules.Hive.Editor
{
    public struct PlatformAlias
    {
        public static PlatformAlias Android => new PlatformAlias("Android", "Android");
        public static PlatformAlias Amazon => new PlatformAlias("Amazon", "Amazon");
        public static PlatformAlias GooglePlay => new PlatformAlias("GooglePlay", "GooglePlay");
        public static PlatformAlias Ios => new PlatformAlias("iOS", "iOS");

        public static bool operator ==(PlatformAlias first, PlatformAlias second) => first.Name == second.Name;

        public static bool operator !=(PlatformAlias first, PlatformAlias second) => !(first == second);

        /// <summary>
        /// Gets a human-readable name of the platform alias.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets a directory name associated with the platform alias.
        /// </summary>
        public string DirectoryName { get; }

        private PlatformAlias(string name, string directoryName)
        {
            Name = name;
            DirectoryName = directoryName;
        }
    }
}