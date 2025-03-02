using System;


namespace Modules.Hive.Editor.BuildUtilities
{
    public struct EnvironmentParameters
    {
        public string this[string parameterName] => Environment.GetEnvironmentVariable(parameterName);
    }
}