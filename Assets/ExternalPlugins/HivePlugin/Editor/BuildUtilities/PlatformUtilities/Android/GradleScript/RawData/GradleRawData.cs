namespace Modules.Hive.Editor.BuildUtilities.Android
{
    public class GradleRawData : IGradleScriptElement
    {
        public string Data { get; }


        public GradleRawData(string data)
        {
            Data = data;
        }


        public void Validate() { }


        public override string ToString() => Data;


        public string ToString(IReadOnlyGradleScript gradleScript) => Data;
    }
}
