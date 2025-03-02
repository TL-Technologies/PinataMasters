using UnityEngine;
using System.Collections.Generic;


namespace MiniJSON 
{
	public class JSONKeyFrameConverter : JSONCustomConverter 
	{
		#region implemented abstract members of JSONCustomConverter

		public override bool IsCanBeDeserialized(string type)
		{
			return type.Equals("Keyframe");
		}


		public override bool IsCanBeDeserialized(System.Type type)
		{
			return type.Equals(typeof(Keyframe));
		}


		public override object Serialize(object obj)
		{
			Keyframe frame = (Keyframe)obj;
			var dict = new Dictionary<string, object>();

			dict.Add("inTangent", frame.inTangent);
			dict.Add("outTangent", frame.outTangent);
			dict.Add("inWeight", frame.inWeight);
			dict.Add("outWeight", frame.outWeight);
			dict.Add("weightedMode", (System.Int16)(frame.weightedMode));
			dict.Add("time", frame.time);
			dict.Add("value", frame.value);			

			return dict;
		}


		public override object Deserialize(object obj)
		{
			Dictionary<string, object> dict = obj as Dictionary<string, object>;

			Keyframe key = new Keyframe();
			key.inTangent = System.Convert.ToSingle(dict["inTangent"]);
			key.outTangent = System.Convert.ToSingle(dict["outTangent"]);
			key.inWeight = System.Convert.ToSingle(dict["inWeight"]);
			key.outWeight = System.Convert.ToSingle(dict["outWeight"]);
			key.weightedMode = (WeightedMode)(System.Convert.ToInt16(dict["weightedMode"]));
			key.time = System.Convert.ToSingle(dict["time"]);
			key.value = System.Convert.ToSingle(dict["value"]);			

			return key;
		}

		#endregion


	}
}