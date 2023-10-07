using System;
using UnityEngine;

// Token: 0x020000F3 RID: 243
public class TransformReset : MonoBehaviour
{
	// Token: 0x060005AA RID: 1450 RVA: 0x00023554 File Offset: 0x00021754
	private void Awake()
	{
		Transform[] componentsInChildren = base.GetComponentsInChildren<Transform>();
		this.transformList = new TransformReset.OriginalGameObjectTransform[componentsInChildren.Length];
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			this.transformList[i] = new TransformReset.OriginalGameObjectTransform(componentsInChildren[i]);
		}
		this.ResetTransforms();
	}

	// Token: 0x060005AB RID: 1451 RVA: 0x000235A0 File Offset: 0x000217A0
	public void ReturnTransforms()
	{
		foreach (TransformReset.OriginalGameObjectTransform originalGameObjectTransform in this.tempTransformList)
		{
			originalGameObjectTransform.thisTransform.position = originalGameObjectTransform.thisPosition;
			originalGameObjectTransform.thisTransform.rotation = originalGameObjectTransform.thisRotation;
		}
	}

	// Token: 0x060005AC RID: 1452 RVA: 0x000235F0 File Offset: 0x000217F0
	public void SetScale(float ratio)
	{
		foreach (TransformReset.OriginalGameObjectTransform originalGameObjectTransform in this.transformList)
		{
			originalGameObjectTransform.thisTransform.localScale *= ratio;
		}
	}

	// Token: 0x060005AD RID: 1453 RVA: 0x00023634 File Offset: 0x00021834
	public void ResetTransforms()
	{
		this.tempTransformList = new TransformReset.OriginalGameObjectTransform[this.transformList.Length];
		for (int i = 0; i < this.transformList.Length; i++)
		{
			this.tempTransformList[i] = new TransformReset.OriginalGameObjectTransform(this.transformList[i].thisTransform);
		}
		foreach (TransformReset.OriginalGameObjectTransform originalGameObjectTransform in this.transformList)
		{
			originalGameObjectTransform.thisTransform.position = originalGameObjectTransform.thisPosition;
			originalGameObjectTransform.thisTransform.rotation = originalGameObjectTransform.thisRotation;
		}
	}

	// Token: 0x0400069D RID: 1693
	private TransformReset.OriginalGameObjectTransform[] transformList;

	// Token: 0x0400069E RID: 1694
	private TransformReset.OriginalGameObjectTransform[] tempTransformList;

	// Token: 0x020003F0 RID: 1008
	private struct OriginalGameObjectTransform
	{
		// Token: 0x06001BD6 RID: 7126 RVA: 0x00096514 File Offset: 0x00094714
		public OriginalGameObjectTransform(Transform constructionTransform)
		{
			this._thisTransform = constructionTransform;
			this._thisPosition = constructionTransform.position;
			this._thisRotation = constructionTransform.rotation;
		}

		// Token: 0x170001FF RID: 511
		// (get) Token: 0x06001BD7 RID: 7127 RVA: 0x00096535 File Offset: 0x00094735
		// (set) Token: 0x06001BD8 RID: 7128 RVA: 0x0009653D File Offset: 0x0009473D
		public Transform thisTransform
		{
			get
			{
				return this._thisTransform;
			}
			set
			{
				this._thisTransform = value;
			}
		}

		// Token: 0x17000200 RID: 512
		// (get) Token: 0x06001BD9 RID: 7129 RVA: 0x00096546 File Offset: 0x00094746
		// (set) Token: 0x06001BDA RID: 7130 RVA: 0x0009654E File Offset: 0x0009474E
		public Vector3 thisPosition
		{
			get
			{
				return this._thisPosition;
			}
			set
			{
				this._thisPosition = value;
			}
		}

		// Token: 0x17000201 RID: 513
		// (get) Token: 0x06001BDB RID: 7131 RVA: 0x00096557 File Offset: 0x00094757
		// (set) Token: 0x06001BDC RID: 7132 RVA: 0x0009655F File Offset: 0x0009475F
		public Quaternion thisRotation
		{
			get
			{
				return this._thisRotation;
			}
			set
			{
				this._thisRotation = value;
			}
		}

		// Token: 0x04001C71 RID: 7281
		private Transform _thisTransform;

		// Token: 0x04001C72 RID: 7282
		private Vector3 _thisPosition;

		// Token: 0x04001C73 RID: 7283
		private Quaternion _thisRotation;
	}
}
