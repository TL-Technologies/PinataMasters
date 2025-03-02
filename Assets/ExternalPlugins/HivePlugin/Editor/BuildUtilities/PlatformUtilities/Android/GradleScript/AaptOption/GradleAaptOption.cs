using System;


namespace Modules.Hive.Editor.BuildUtilities.Android
{
    public class GradleAaptOption : IGradleScriptElement
    {
        public string Key { get; }
        public string Value { get; }

        public GradleAaptOption(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public void Validate()
        {
            if (string.IsNullOrEmpty(Key) || string.IsNullOrEmpty(Value))
            {
                throw new ArgumentNullException(
                    $"Trying to add incorrect AaptOption to gradle, both values shouldn't be empty {Key} = {Value}");
            }
        }

        public string ToString(IReadOnlyGradleScript gradleScript) => $"{Key} = {Value}";
    }
}