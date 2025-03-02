namespace Modules.Hive.Editor.BuildUtilities.Android
{
    public interface IGradleScriptElement
    {
        void Validate();
        string ToString();
        string ToString(IReadOnlyGradleScript gradleScript);
    }
}
