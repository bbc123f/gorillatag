using System;
using UnityEngine;

// Token: 0x020000F3 RID: 243
public class TransformReset : MonoBehaviour
{
	// Token: 0x060005AA RID: 1450 RVA: 0x00023348 File Offset: 0x00021548
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

	// Token: 0x060005AB RID: 1451 RVA: 0x00023394 File Offset: 0x00021594
	public void ReturnTransforms()
	{
		foreach (TransformReset.OriginalGameObjectTransform originalGameObjectTransform in this.tempTransformList)
		{
			originalGameObjectTransform.thisTransform.position = originalGameObjectTransform.thisPosition;
			originalGameObjectTransform.thisTransform.rotation = originalGameObjectTransform.thisRotation;
		}
	}

	// Token: 0x060005AC RID: 1452 RVA: 0x000233E4 File Offset: 0x000215E4
	public void SetScale(float ratio)
	{
		foreach (TransformReset.OriginalGameObjectTransform originalGameObjectTransform in this.transformList)
		{
			originalGameObjectTransform.thisTransform.localScale *= ratio;
		}
	}

	// Token: 0x060005AD RID: 1453 RVA: 0x00023428 File Offset: 0x00021628
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

	// Token: 0x020003F2 RID: 1010
	private struct OriginalGameObjectTransform
	{
		// Token: 0x06001BDF RID: 7135 RVA: 0x000969FC File Offset: 0x00094BFC
		public OriginalGameObjectTransform(Transform constructionTransform)
		{
			this._thisTransform = constructionTransform;
			this._thisPosition = constructionTransform.position;
			this._thisRotation = constructionTransform.rotation;
		}

		// Token: 0x17000201 RID: 513
		// (get) Token: 0x06001BE0 RID: 7136 RVA: 0x00096A1D File Offset: 0x00094C1D
		// (set) Token: 0x06001BE1 RID: 7137 RVA: 0x00096A25 File Offset: 0x00094C25
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

		// Token: 0x17000202 RID: 514
		// (get) Token: 0x06001BE2 RID: 7138 RVA: 0x00096A2E File Offset: 0x00094C2E
		// (set) Token: 0x06001BE3 RID: 7139 RVA: 0x00096A36 File Offset: 0x00094C36
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

		// Token: 0x17000203 RID: 515
		// (get) Token: 0x06001BE4 RID: 7140 RVA: 0x00096A3F File Offset: 0x00094C3F
		// (set) Token: 0x06001BE5 RID: 7141 RVA: 0x00096A47 File Offset: 0x00094C47
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

		// Token: 0x04001C7E RID: 7294
		private Transform _thisTransform;

		// Token: 0x04001C7F RID: 7295
		private Vector3 _thisPosition;

		// Token: 0x04001C80 RID: 7296
		private Quaternion _thisRotation;
	}
}
