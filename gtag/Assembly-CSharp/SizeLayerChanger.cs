using System;
using UnityEngine;

// Token: 0x0200019A RID: 410
public class SizeLayerChanger : MonoBehaviour
{
	// Token: 0x1700007A RID: 122
	// (get) Token: 0x06000A95 RID: 2709 RVA: 0x0004186C File Offset: 0x0003FA6C
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

	// Token: 0x06000A96 RID: 2710 RVA: 0x000418AC File Offset: 0x0003FAAC
	private void Awake()
	{
		this.minScale = Mathf.Max(this.minScale, 0.01f);
	}

	// Token: 0x06000A97 RID: 2711 RVA: 0x000418C4 File Offset: 0x0003FAC4
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

	// Token: 0x06000A98 RID: 2712 RVA: 0x0004190B File Offset: 0x0003FB0B
	public void OnTriggerExit(Collider other)
	{
		if (!other.GetComponent<SphereCollider>())
		{
			return;
		}
		other.attachedRigidbody.gameObject.GetComponent<VRRig>() == null;
	}

	// Token: 0x04000D69 RID: 3433
	public float maxScale;

	// Token: 0x04000D6A RID: 3434
	public float minScale;

	// Token: 0x04000D6B RID: 3435
	public bool isAssurance;

	// Token: 0x04000D6C RID: 3436
	public bool affectLayerA = true;

	// Token: 0x04000D6D RID: 3437
	public bool affectLayerB = true;

	// Token: 0x04000D6E RID: 3438
	public bool affectLayerC = true;

	// Token: 0x04000D6F RID: 3439
	public bool affectLayerD = true;
}
