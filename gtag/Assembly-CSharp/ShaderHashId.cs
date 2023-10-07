using System;
using UnityEngine;

// Token: 0x0200021C RID: 540
public struct ShaderHashId
{
	// Token: 0x06000D6B RID: 3435 RVA: 0x0004E3D6 File Offset: 0x0004C5D6
	public ShaderHashId(string hashText)
	{
		this.hashText = hashText;
		this.hashValue = Shader.PropertyToID(hashText);
	}

	// Token: 0x06000D6C RID: 3436 RVA: 0x0004E3EB File Offset: 0x0004C5EB
	public static implicit operator int(ShaderHashId h)
	{
		return h.hashValue;
	}

	// Token: 0x06000D6D RID: 3437 RVA: 0x0004E3F3 File Offset: 0x0004C5F3
	public static implicit operator ShaderHashId(string s)
	{
		return new ShaderHashId(s);
	}

	// Token: 0x04001086 RID: 4230
	private string hashText;

	// Token: 0x04001087 RID: 4231
	private int hashValue;
}
