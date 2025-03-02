namespace Modules.Hive.Editor.BuildUtilities.Ios
{
    public struct LinkerFlag
    {
        public static LinkerFlag Arc => new LinkerFlag("-fobjc-arc", LinkerFlagScope.Project | LinkerFlagScope.File);
        public static LinkerFlag Cpp => new LinkerFlag("-lc++", LinkerFlagScope.Project);
        public static LinkerFlag ObjC => new LinkerFlag("-ObjC", LinkerFlagScope.Project | LinkerFlagScope.File);
        public static LinkerFlag AllLoad => new LinkerFlag("-all_load", LinkerFlagScope.Project | LinkerFlagScope.File);
        public static LinkerFlag NoArc => new LinkerFlag("-fno-objc-arc", LinkerFlagScope.File);
        public static LinkerFlag Z => new LinkerFlag("-lz", LinkerFlagScope.Project);


        /// <summary>
        /// Gets the linker flag.
        /// </summary>
        public string Flag { get; }


        /// <summary>
        /// Gets a scope of the linker flag.
        /// </summary>
        public LinkerFlagScope Scope { get; }


        private LinkerFlag(string flag, LinkerFlagScope scope)
        {
            Flag = flag;
            Scope = scope;
        }
    }
}
