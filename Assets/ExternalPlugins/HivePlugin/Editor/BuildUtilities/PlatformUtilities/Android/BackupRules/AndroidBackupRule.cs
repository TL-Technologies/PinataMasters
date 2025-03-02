using System;


namespace Modules.Hive.Editor.BuildUtilities.Android
{
    public class AndroidBackupRule
    {
        public readonly AndroidBackupRuleType type;
        public readonly AndroidBackupDomain domain;
        public readonly string path;


        public AndroidBackupRule(AndroidBackupRuleType type, AndroidBackupDomain domain, string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException($"Argument {nameof(path)} should not be null or empty.");
            }

            this.type = type;
            this.domain = domain;
            this.path = path;
        }
    }
}
