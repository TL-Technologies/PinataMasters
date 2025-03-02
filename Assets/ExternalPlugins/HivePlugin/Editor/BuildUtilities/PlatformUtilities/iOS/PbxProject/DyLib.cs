namespace Modules.Hive.Editor.BuildUtilities.Ios
{
    public struct DyLib
    {
        public static DyLib LibCpp         => new DyLib("libc++.tbd");
        public static DyLib LibIcuCore     => new DyLib("libicucore.tbd");
        public static DyLib LibResolv      => new DyLib("libresolv.tbd");
        public static DyLib LibSqlite3     => new DyLib("libsqlite3.tbd");
        public static DyLib LibXml2        => new DyLib("libxml2.tbd");
        public static DyLib LibXml2Dot2    => new DyLib("libxml2.2.tbd");
        public static DyLib LibZ           => new DyLib("libz.tbd");
        public static DyLib LibBz2         => new DyLib("libbz2.tbd");
        public static DyLib LibCppAbi      => new DyLib("libc++abi.tbd");


        /// <summary>
        /// Gets a reference to the dynamic library.
        /// </summary>
        public string Reference { get; }


        private DyLib(string reference)
        {
            Reference = reference;
        }
    }
}
