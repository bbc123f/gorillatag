using System;
using UnityEngine;

public static class EchoUtils
{
	public static T Echo<T>(this T value)
	{
		Debug.Log(value);
		return value;
	}
}
