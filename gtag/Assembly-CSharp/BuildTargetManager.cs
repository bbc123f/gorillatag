using System;
using UnityEngine;

// Token: 0x02000200 RID: 512
public class BuildTargetManager : MonoBehaviour
{
	// Token: 0x06000D1B RID: 3355 RVA: 0x0004D583 File Offset: 0x0004B783
	public string GetPath()
	{
		return this.path;
	}

	// Token: 0x04001040 RID: 4160
	public BuildTargetManager.BuildTowards newBuildTarget;

	// Token: 0x04001041 RID: 4161
	public bool isBeta;

	// Token: 0x04001042 RID: 4162
	public bool isQA;

	// Token: 0x04001043 RID: 4163
	public bool spoofIDs;

	// Token: 0x04001044 RID: 4164
	public bool enableAllCosmetics;

	// Token: 0x04001045 RID: 4165
	public OVRManager ovrManager;

	// Token: 0x04001046 RID: 4166
	private string path = "Assets/csc.rsp";

	// Token: 0x04001047 RID: 4167
	public BuildTargetManager.BuildTowards currentBuildTargetDONOTCHANGE;

	// Token: 0x04001048 RID: 4168
	public GorillaTagger gorillaTagger;

	// Token: 0x04001049 RID: 4169
	public GameObject[] betaDisableObjects;

	// Token: 0x0400104A RID: 4170
	public GameObject[] betaEnableObjects;

	// Token: 0x02000471 RID: 1137
	public enum BuildTowards
	{
		// Token: 0x04001E8C RID: 7820
		Steam,
		// Token: 0x04001E8D RID: 7821
		OculusPC,
		// Token: 0x04001E8E RID: 7822
		Quest,
		// Token: 0x04001E8F RID: 7823
		Viveport
	}
}
