using System;
using UnityEngine;

// Token: 0x0200019B RID: 411
public class SizeLayerChanger : MonoBehaviour
{
	// Token: 0x1700007C RID: 124
	// (get) Token: 0x06000A9A RID: 2714 RVA: 0x000419A4 File Offset: 0x0003FBA4
	public int SizeLayerMask
	{
		get
		{
			int num = 0;
			if (this.affectLayerA)
			{
				num |= 1;
			}
			if (this.affectLayerB)
			{
				num |= 2;
			}
			if (this.affectLayerC)
			{
				num |= 4;
			}
			if (this.affectLayerD)
			{
				num |= 8;
			}
			return num;
		}
	}

	// Token: 0x06000A9B RID: 2715 RVA: 0x000419E4 File Offset: 0x0003FBE4
	private void Awake()
	{
		this.minScale = Mathf.Max(this.minScale, 0.01f);
	}

	// Token: 0x06000A9C RID: 2716 RVA: 0x000419FC File Offset: 0x0003FBFC
	public void OnTriggerEnter(Collider other)
	{
		if (!other.GetComponent<SphereCollider>())
		{
			return;
		}
		VRRig component = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		if (component == null)
		{
			return;
		}
		component.sizeManager.currentSizeLayerMaskValue = this.SizeLayerMask;
	}

	// Token: 0x06000A9D RID: 2717 RVA: 0x00041A43 File Offset: 0x0003FC43
	public void OnTriggerExit(Collider other)
	{
		if (!other.GetComponent<SphereCollider>())
		{
			return;
		}
		other.attachedRigidbody.gameObject.GetComponent<VRRig>() == null;
	}

	// Token: 0x04000D6D RID: 3437
	public float maxScale;

	// Token: 0x04000D6E RID: 3438
	public float minScale;

	// Token: 0x04000D6F RID: 3439
	public bool isAssurance;

	// Token: 0x04000D70 RID: 3440
	public bool affectLayerA = true;

	// Token: 0x04000D71 RID: 3441
	public bool affectLayerB = true;

	// Token: 0x04000D72 RID: 3442
	public bool affectLayerC = true;

	// Token: 0x04000D73 RID: 3443
	public bool affectLayerD = true;
}
