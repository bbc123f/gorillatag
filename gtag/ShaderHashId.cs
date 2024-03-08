using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public struct ShaderHashId
{
	public ShaderHashId(string text)
	{
		this._text = text;
		this._hash = Shader.PropertyToID(text);
	}

	public static implicit operator int(ShaderHashId h)
	{
		return h._hash;
	}

	public static implicit operator ShaderHashId(string s)
	{
		return new ShaderHashId(s);
	}

	[FormerlySerializedAs("_hashText")]
	[SerializeField]
	private string _text;

	[DebugReadOnly]
	[NonSerialized]
	private int _hash;
}
