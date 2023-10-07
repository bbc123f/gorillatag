using System;
using UnityEngine;

// Token: 0x02000069 RID: 105
public static class ShaderPlatformSetter
{
	// Token: 0x060001F9 RID: 505 RVA: 0x0000DEC2 File Offset: 0x0000C0C2
	[RuntimeInitializeOnLoadMethod]
	public static void HandleRuntimeInitializeOnLoad()
	{
		Shader.DisableKeyword("PLATFORM_IS_ANDROID");
		Shader.DisableKeyword("QATESTING");
	}
}
