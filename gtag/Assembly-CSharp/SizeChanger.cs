using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000199 RID: 409
public class SizeChanger : GorillaTriggerBox
{
	// Token: 0x17000079 RID: 121
	// (get) Token: 0x06000A90 RID: 2704 RVA: 0x00041720 File Offset: 0x0003F920
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

	// Token: 0x06000A91 RID: 2705 RVA: 0x00041760 File Offset: 0x0003F960
	private void Awake()
	{
		this.minScale = Mathf.Max(this.minScale, 0.01f);
		this.myCollider = base.GetComponent<Collider>();
	}

	// Token: 0x06000A92 RID: 2706 RVA: 0x00041784 File Offset: 0x0003F984
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

	// Token: 0x06000A93 RID: 2707 RVA: 0x000417E0 File Offset: 0x0003F9E0
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

	// Token: 0x04000D56 RID: 3414
	public VRRig rigRef;

	// Token: 0x04000D57 RID: 3415
	public SizeChanger.ChangerType myType;

	// Token: 0x04000D58 RID: 3416
	public float maxScale;

	// Token: 0x04000D59 RID: 3417
	public float minScale;

	// Token: 0x04000D5A RID: 3418
	public Collider myCollider;

	// Token: 0x04000D5B RID: 3419
	public float insideThreshold = 0.01f;

	// Token: 0x04000D5C RID: 3420
	public List<VRRig> insideRigs;

	// Token: 0x04000D5D RID: 3421
	public List<VRRig> leftRigs;

	// Token: 0x04000D5E RID: 3422
	public float scaleLerp;

	// Token: 0x04000D5F RID: 3423
	public Transform startPos;

	// Token: 0x04000D60 RID: 3424
	public Transform endPos;

	// Token: 0x04000D61 RID: 3425
	public int priority;

	// Token: 0x04000D62 RID: 3426
	public bool aprilFoolsEnabled;

	// Token: 0x04000D63 RID: 3427
	public float startRadius;

	// Token: 0x04000D64 RID: 3428
	public float endRadius;

	// Token: 0x04000D65 RID: 3429
	public bool affectLayerA = true;

	// Token: 0x04000D66 RID: 3430
	public bool affectLayerB = true;

	// Token: 0x04000D67 RID: 3431
	public bool affectLayerC = true;

	// Token: 0x04000D68 RID: 3432
	public bool affectLayerD = true;

	// Token: 0x02000443 RID: 1091
	public enum ChangerType
	{
		// Token: 0x04001DA7 RID: 7591
		Static,
		// Token: 0x04001DA8 RID: 7592
		Continuous,
		// Token: 0x04001DA9 RID: 7593
		Radius
	}
}
