namespace Modules.Hive.Editor.BuildUtilities.Android
{
    public class GradleConfiguration : IGradleScriptElement
    {
        public string Data { get; }
        
        
        public GradleConfiguration(string data)
        {
            Data = data;
        }
        
        
        public void Validate() { }
        public string ToString(IReadOnlyGradleScript gradleScript) => Data;

        
        public override string ToString() => Data;
    }
}