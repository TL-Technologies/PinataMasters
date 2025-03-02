using System;


namespace Modules.Hive.Editor.BuildUtilities.Ios
{
    public struct ApsEnvironment
    {
        public static ApsEnvironment Development => new ApsEnvironment("development");
        public static ApsEnvironment Production => new ApsEnvironment("production");
        

        public string Name { get; }


        internal ApsEnvironment(string name)
        {
            Name = name;
        }

        public override bool Equals(object obj)
        {
            return obj is ApsEnvironment env &&
                string.Equals(Name, env.Name, StringComparison.OrdinalIgnoreCase);
        }


        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }


        public static bool operator ==(ApsEnvironment a, ApsEnvironment b) => 
            string.Equals(a.Name, b.Name, StringComparison.OrdinalIgnoreCase);


        public static bool operator !=(ApsEnvironment a, ApsEnvironment b) => !(a == b);
    }
}
