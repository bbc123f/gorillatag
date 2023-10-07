using System;
using UnityEngine;

// Token: 0x02000027 RID: 39
public class Monkeye_LazerFX : MonoBehaviour
{
	// Token: 0x060000DE RID: 222 RVA: 0x00008D28 File Offset: 0x00006F28
	private void Awake()
	{
		base.enabled = false;
	}

	// Token: 0x060000DF RID: 223 RVA: 0x00008D34 File Offset: 0x00006F34
	public void EnableLazer(Transform[] eyes_, VRRig rig_)
	{
		if (rig_ == this.rig)
		{
			return;
		}
		this.eyeBones = eyes_;
		this.rig = rig_;
		base.enabled = true;
		LineRenderer[] array = this.lines;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].positionCount = 2;
		}
	}

	// Token: 0x060000E0 RID: 224 RVA: 0x00008D84 File Offset: 0x00006F84
	public void DisableLazer()
	{
		if (base.enabled)
		{
			base.enabled = false;
			LineRenderer[] array = this.lines;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].positionCount = 0;
			}
		}
	}

	// Token: 0x060000E1 RID: 225 RVA: 0x00008DC0 File Offset: 0x00006FC0
	private void Update()
	{
		for (int i = 0; i < this.lines.Length; i++)
		{
			this.lines[i].SetPosition(0, this.eyeBones[i].transform.position);
			this.lines[i].SetPosition(1, this.rig.transform.position);
		}
	}

	// Token: 0x0400012C RID: 300
	private Transform[] eyeBones;

	// Token: 0x0400012D RID: 301
	private VRRig rig;

	// Token: 0x0400012E RID: 302
	public LineRenderer[] lines;
}
