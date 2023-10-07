using System;
using UnityEngine;

// Token: 0x0200011E RID: 286
public class TransferrableObjectGripPosition : MonoBehaviour
{
	// Token: 0x0600078F RID: 1935 RVA: 0x000308BE File Offset: 0x0002EABE
	private void Awake()
	{
		if (this.parentObject == null)
		{
			this.parentObject = base.transform.parent.GetComponent<TransferrableItemSlotTransformOverride>();
		}
		this.parentObject.AddGripPosition(this.attachmentType, this);
	}

	// Token: 0x06000790 RID: 1936 RVA: 0x000308F6 File Offset: 0x0002EAF6
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
