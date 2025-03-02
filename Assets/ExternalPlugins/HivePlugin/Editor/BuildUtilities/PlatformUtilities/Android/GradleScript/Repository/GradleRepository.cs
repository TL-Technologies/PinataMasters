using System;


namespace Modules.Hive.Editor.BuildUtilities.Android
{
    public class GradleRepository : IGradleScriptElement
    {
        public static readonly GradleRepository JCenter = new GradleRepository("jcenter()");
        public static readonly GradleRepository Google = new GradleRepository("google()");
        public static readonly GradleRepository MavenCentral = new GradleRepository("mavenCentral()");


        public string Reference { get; }


        public GradleRepository(string reference)
        {
            Reference = reference;
        }


        public void Validate()
        {
            if (string.IsNullOrEmpty(Reference))
            {
                throw new FormatException("Missing repository reference in GradleRepository element");
            }
        }


        public override string ToString() => Reference;


        public string ToString(IReadOnlyGradleScript gradleScript) => Reference;
    }
}
