using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200019A RID: 410
public class SizeChanger : GorillaTriggerBox
{
	// Token: 0x1700007B RID: 123
	// (get) Token: 0x06000A95 RID: 2709 RVA: 0x00041858 File Offset: 0x0003FA58
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

	// Token: 0x06000A96 RID: 2710 RVA: 0x00041898 File Offset: 0x0003FA98
	private void Awake()
	{
		this.minScale = Mathf.Max(this.minScale, 0.01f);
		this.myCollider = base.GetComponent<Collider>();
	}

	// Token: 0x06000A97 RID: 2711 RVA: 0x000418BC File Offset: 0x0003FABC
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
		if (!component.sizeManager.touchingChangers.Contains(this))
		{
			component.sizeManager.touchingChangers.Add(this);
		}
	}

	// Token: 0x06000A98 RID: 2712 RVA: 0x00041918 File Offset: 0x0003FB18
	public void OnTriggerExit(Collider other)
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
		if (component.sizeManager.touchingChangers.Contains(this))
		{
			component.sizeManager.touchingChangers.Remove(this);
		}
	}

	// Token: 0x04000D5A RID: 3418
	public VRRig rigRef;

	// Token: 0x04000D5B RID: 3419
	public SizeChanger.ChangerType myType;

	// Token: 0x04000D5C RID: 3420
	public float maxScale;

	// Token: 0x04000D5D RID: 3421
	public float minScale;

	// Token: 0x04000D5E RID: 3422
	public Collider myCollider;

	// Token: 0x04000D5F RID: 3423
	public float insideThreshold = 0.01f;

	// Token: 0x04000D60 RID: 3424
	public List<VRRig> insideRigs;

	// Token: 0x04000D61 RID: 3425
	public List<VRRig> leftRigs;

	// Token: 0x04000D62 RID: 3426
	public float scaleLerp;

	// Token: 0x04000D63 RID: 3427
	public Transform startPos;

	// Token: 0x04000D64 RID: 3428
	public Transform endPos;

	// Token: 0x04000D65 RID: 3429
	public int priority;

	// Token: 0x04000D66 RID: 3430
	public bool aprilFoolsEnabled;

	// Token: 0x04000D67 RID: 3431
	public float startRadius;

	// Token: 0x04000D68 RID: 3432
	public float endRadius;

	// Token: 0x04000D69 RID: 3433
	public bool affectLayerA = true;

	// Token: 0x04000D6A RID: 3434
	public bool affectLayerB = true;

	// Token: 0x04000D6B RID: 3435
	public bool affectLayerC = true;

	// Token: 0x04000D6C RID: 3436
	public bool affectLayerD = true;

	// Token: 0x02000445 RID: 1093
	public enum ChangerType
	{
		// Token: 0x04001DB4 RID: 7604
		Static,
		// Token: 0x04001DB5 RID: 7605
		Continuous,
		// Token: 0x04001DB6 RID: 7606
		Radius
	}
}
