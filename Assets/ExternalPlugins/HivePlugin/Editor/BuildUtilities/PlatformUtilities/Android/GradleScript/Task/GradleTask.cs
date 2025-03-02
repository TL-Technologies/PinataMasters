namespace Modules.Hive.Editor.BuildUtilities.Android
{
    public abstract class GradleTask : IGradleScriptElement
    {
        public string Identifier { get; }


        public GradleTask(string identifier)
        {
            Identifier = identifier;
        }


        public abstract void Validate();


        public abstract string ToString(IReadOnlyGradleScript gradleScript);


        public bool HasIdentifier => string.IsNullOrEmpty(Identifier);
    }
}
