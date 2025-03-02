using System.Collections.Generic;


namespace Modules.Hive.Editor.BuildUtilities.Ios.Stub
{
    public class PlistElementArray : PlistElement
    {
		public List<PlistElement> values = new List<PlistElement>();

		public void AddString(string val) { }

		public PlistElementDict AddDict() { return null; }
	}
}