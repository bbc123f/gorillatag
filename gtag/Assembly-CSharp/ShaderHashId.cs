using System;
using UnityEngine;

// Token: 0x0200021D RID: 541
public struct ShaderHashId
{
	// Token: 0x06000D71 RID: 3441 RVA: 0x0004E636 File Offset: 0x0004C836
	public ShaderHashId(string hashText)
	{
		this.hashText = hashText;
		this.hashValue = Shader.PropertyToID(hashText);
	}

	// Token: 0x06000D72 RID: 3442 RVA: 0x0004E64B File Offset: 0x0004C84B
	public static implicit operator int(ShaderHashId h)
	{
		return h.hashValue;
	}

	// Token: 0x06000D73 RID: 3443 RVA: 0x0004E653 File Offset: 0x0004C853
	public static implicit operator ShaderHashId(string s)
	{
		return new ShaderHashId(s);
	}

	// Token: 0x0400108B RID: 4235
	private string hashText;

	// Token: 0x0400108C RID: 4236
	private int hashValue;
}
