using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils {

	public static T RandomEnum<T>(int beginOffset = 0, int endOffset = 0) {
		Array values = Enum.GetValues(typeof(T));
		int index = UnityEngine.Random.Range(beginOffset, values.Length - endOffset);
		return (T) values.GetValue(index);
	}
}
