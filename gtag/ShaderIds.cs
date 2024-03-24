using System;
using UnityEngine;

public static class ShaderIds
{
	// Note: this type is marked as 'beforefieldinit'.
	static ShaderIds()
	{
	}

	public static readonly int _MainTex = Shader.PropertyToID("_MainTex");

	public static readonly int _EmissionColor = Shader.PropertyToID("_EmissionColor");

	public static readonly int _Color = Shader.PropertyToID("_Color");
}
