using System.Collections.Generic;


namespace Modules.Hive.Editor.BuildUtilities.Ios.Stub
{
    public class PlistElementDict : PlistElement
	{
		private SortedDictionary<string, PlistElement> m_PrivateValue = new SortedDictionary<string, PlistElement>();

		public IDictionary<string, PlistElement> values => m_PrivateValue;

		public new PlistElement this[string key]
		{
			get { return null; }
			set { }
		}

		public void SetInteger(string key, int val) { }

		public void SetString(string key, string val) { }

		public void SetBoolean(string key, bool val) { }

		public PlistElementArray CreateArray(string key) { return null; }

		public PlistElementDict CreateDict(string key) { return null; }
	}
}