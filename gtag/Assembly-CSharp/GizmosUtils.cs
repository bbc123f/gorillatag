using System;
using System.Diagnostics;
using UnityEngine;

public static class GizmosUtils
{
	[Conditional("UNITY_EDITOR")]
	public static void DrawCubeTRS(Vector3 t, Quaternion r, Vector3 s)
	{
	}

	[Conditional("UNITY_EDITOR")]
	public static void DrawWireCubeTRS(Vector3 t, Quaternion r, Vector3 s)
	{
	}
}
