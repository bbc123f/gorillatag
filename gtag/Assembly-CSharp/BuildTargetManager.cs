using System;
using UnityEngine;

// Token: 0x020001FF RID: 511
public class BuildTargetManager : MonoBehaviour
{
	// Token: 0x06000D15 RID: 3349 RVA: 0x0004D323 File Offset: 0x0004B523
	public string GetPath()
	{
		return this.path;
	}

	// Token: 0x0400103B RID: 4155
	public BuildTargetManager.BuildTowards newBuildTarget;

	// Token: 0x0400103C RID: 4156
	public bool isBeta;

	// Token: 0x0400103D RID: 4157
	public bool isQA;

	// Token: 0x0400103E RID: 4158
	public bool spoofIDs;

	// Token: 0x0400103F RID: 4159
	public bool enableAllCosmetics;

	// Token: 0x04001040 RID: 4160
	public OVRManager ovrManager;

	// Token: 0x04001041 RID: 4161
	private string path = "Assets/csc.rsp";

	// Token: 0x04001042 RID: 4162
	public BuildTargetManager.BuildTowards currentBuildTargetDONOTCHANGE;

	// Token: 0x04001043 RID: 4163
	public GorillaTagger gorillaTagger;

	// Token: 0x04001044 RID: 4164
	public GameObject[] betaDisableObjects;

	// Token: 0x04001045 RID: 4165
	public GameObject[] betaEnableObjects;

	// Token: 0x0200046F RID: 1135
	public enum BuildTowards
	{
		// Token: 0x04001E7F RID: 7807
		Steam,
		// Token: 0x04001E80 RID: 7808
		OculusPC,
		// Token: 0x04001E81 RID: 7809
		Quest,
		// Token: 0x04001E82 RID: 7810
		Viveport
	}
}
