using System;
using UnityEngine;

public struct ShaderHashId
{
	public ShaderHashId(string hashText)
	{
		this.hashText = hashText;
		this.hashValue = Shader.PropertyToID(hashText);
	}

	public static implicit operator int(ShaderHashId h)
	{
		return h.hashValue;
	}

	public static implicit operator ShaderHashId(string s)
	{
		return new ShaderHashId(s);
	}

	private string hashText;

	private int hashValue;
}
