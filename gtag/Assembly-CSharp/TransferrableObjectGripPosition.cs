using System;
using UnityEngine;

// Token: 0x0200011E RID: 286
public class TransferrableObjectGripPosition : MonoBehaviour
{
	// Token: 0x06000790 RID: 1936 RVA: 0x000306FE File Offset: 0x0002E8FE
	private void Awake()
	{
		if (this.parentObject == null)
		{
			this.parentObject = base.transform.parent.GetComponent<TransferrableItemSlotTransformOverride>();
		}
		this.parentObject.AddGripPosition(this.attachmentType, this);
	}

	// Token: 0x06000791 RID: 1937 RVA: 0x00030736 File Offset: 0x0002E936
	public SubGrabPoint CreateSubGrabPoint(SlotTransformOverride overrideContainer)
	{
		return new SubGrabPoint();
	}

	// Token: 0x04000931 RID: 2353
	[SerializeField]
	private TransferrableItemSlotTransformOverride parentObject;

	// Token: 0x04000932 RID: 2354
	[SerializeField]
	private TransferrableObject.PositionState attachmentType;
}
