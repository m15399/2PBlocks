using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;

public static class Utils {

	public static void DumpObject<T>(T obj){
		XmlSerializer xmlSerializer = new XmlSerializer(obj.GetType());

		using(StringWriter textWriter = new StringWriter())
		{
			xmlSerializer.Serialize(textWriter, obj);
			Debug.Log(obj.ToString() + " :: " + textWriter.ToString());
		}
	}

	public static T RandomEnum<T>(int beginOffset = 0, int endOffset = 0) {
		Array values = Enum.GetValues(typeof(T));
		int index = UnityEngine.Random.Range(beginOffset, values.Length - endOffset);
		return (T) values.GetValue(index);
	}
}
