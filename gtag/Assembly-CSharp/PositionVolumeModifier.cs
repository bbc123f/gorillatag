using System;
using UnityEngine;

// Token: 0x0200001E RID: 30
public class PositionVolumeModifier : MonoBehaviour
{
	// Token: 0x06000069 RID: 105 RVA: 0x00004AA9 File Offset: 0x00002CA9
	public void OnTriggerStay(Collider other)
	{
		this.audioToMod.isModified = true;
	}

	// Token: 0x04000094 RID: 148
	public TimeOfDayDependentAudio audioToMod;
}
