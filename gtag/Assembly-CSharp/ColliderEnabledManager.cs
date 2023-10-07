using System;
using UnityEngine;

// Token: 0x0200001C RID: 28
public class ColliderEnabledManager : MonoBehaviour
{
	// Token: 0x0600005E RID: 94 RVA: 0x00004836 File Offset: 0x00002A36
	private void Start()
	{
		this.floorEnabled = true;
		this.floorCollidersEnabled = true;
	}

	// Token: 0x0600005F RID: 95 RVA: 0x00004846 File Offset: 0x00002A46
	public void DisableFloorForFrame()
	{
		this.floorEnabled = false;
	}

	// Token: 0x06000060 RID: 96 RVA: 0x00004850 File Offset: 0x00002A50
	private void LateUpdate()
	{
		if (!this.floorEnabled && this.floorCollidersEnabled)
		{
			this.DisableFloor();
		}
		if (!this.floorCollidersEnabled && Time.time > this.timeDisabled + this.disableLength)
		{
			this.floorCollidersEnabled = true;
		}
		Collider[] array = this.floorCollider;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = this.floorCollidersEnabled;
		}
		if (this.floorCollidersEnabled)
		{
			GorillaSurfaceOverride[] array2 = this.walls;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].overrideIndex = this.wallsBeforeMaterial;
			}
		}
		else
		{
			GorillaSurfaceOverride[] array2 = this.walls;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].overrideIndex = this.wallsAfterMaterial;
			}
		}
		this.floorEnabled = true;
	}

	// Token: 0x06000061 RID: 97 RVA: 0x00004910 File Offset: 0x00002B10
	private void DisableFloor()
	{
		this.floorCollidersEnabled = false;
		this.timeDisabled = Time.time;
	}

	// Token: 0x04000088 RID: 136
	public Collider[] floorCollider;

	// Token: 0x04000089 RID: 137
	public bool floorEnabled;

	// Token: 0x0400008A RID: 138
	public bool wasFloorEnabled;

	// Token: 0x0400008B RID: 139
	public bool floorCollidersEnabled;

	// Token: 0x0400008C RID: 140
	[GorillaSoundLookup]
	public int wallsBeforeMaterial;

	// Token: 0x0400008D RID: 141
	[GorillaSoundLookup]
	public int wallsAfterMaterial;

	// Token: 0x0400008E RID: 142
	public GorillaSurfaceOverride[] walls;

	// Token: 0x0400008F RID: 143
	public float timeDisabled;

	// Token: 0x04000090 RID: 144
	public float disableLength;
}
