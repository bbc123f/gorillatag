using System;
using System.Collections.Generic;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200019B RID: 411
public class SizeManager : MonoBehaviour
{
	// Token: 0x1700007B RID: 123
	// (get) Token: 0x06000A9A RID: 2714 RVA: 0x00041956 File Offset: 0x0003FB56
	public float currentScale
	{
		get
		{
			if (this.targetRig != null)
			{
				return this.targetRig.scaleFactor;
			}
			if (this.targetPlayer != null)
			{
				return this.targetPlayer.scale;
			}
			return 1f;
		}
	}

	// Token: 0x1700007C RID: 124
	// (get) Token: 0x06000A9B RID: 2715 RVA: 0x00041991 File Offset: 0x0003FB91
	// (set) Token: 0x06000A9C RID: 2716 RVA: 0x000419C8 File Offset: 0x0003FBC8
	public int currentSizeLayerMaskValue
	{
		get
		{
			if (this.targetPlayer)
			{
				return this.targetPlayer.sizeLayerMask;
			}
			if (this.targetRig)
			{
				return this.targetRig.SizeLayerMask;
			}
			return 1;
		}
		set
		{
			if (this.targetPlayer)
			{
				this.targetPlayer.sizeLayerMask = value;
				if (this.targetRig != null)
				{
					this.targetRig.SizeLayerMask = value;
					return;
				}
			}
			else if (this.targetRig)
			{
				this.targetRig.SizeLayerMask = value;
			}
		}
	}

	// Token: 0x06000A9D RID: 2717 RVA: 0x00041A22 File Offset: 0x0003FC22
	private void OnDisable()
	{
		this.touchingChangers.Clear();
		this.currentSizeLayerMaskValue = 1;
	}

	// Token: 0x06000A9E RID: 2718 RVA: 0x00041A38 File Offset: 0x0003FC38
	private void CollectLineRenderers(GameObject obj)
	{
		this.lineRenderers = obj.GetComponentsInChildren<LineRenderer>(true);
		int num = this.lineRenderers.Length;
		foreach (LineRenderer lineRenderer in this.lineRenderers)
		{
			this.initLineScalar.Add(lineRenderer.widthMultiplier);
		}
	}

	// Token: 0x06000A9F RID: 2719 RVA: 0x00041A88 File Offset: 0x0003FC88
	private void Awake()
	{
		this.rate = 650f;
		if (this.targetRig != null)
		{
			this.CollectLineRenderers(this.targetRig.gameObject);
		}
		else if (this.targetPlayer != null)
		{
			this.CollectLineRenderers(GorillaTagger.Instance.offlineVRRig.gameObject);
		}
		this.mainCameraTransform = Camera.main.transform;
		if (this.targetPlayer != null)
		{
			this.myType = SizeManager.SizeChangerType.LocalOffline;
			return;
		}
		if (this.targetRig != null && !this.targetRig.isOfflineVRRig && this.targetRig.photonView != null && this.targetRig.photonView.Owner != PhotonNetwork.LocalPlayer)
		{
			this.myType = SizeManager.SizeChangerType.OtherOnline;
			return;
		}
		this.myType = SizeManager.SizeChangerType.LocalOnline;
	}

	// Token: 0x06000AA0 RID: 2720 RVA: 0x00041B60 File Offset: 0x0003FD60
	private void FixedUpdate()
	{
		float num = 1f;
		SizeManager.SizeChangerType sizeChangerType = this.myType;
		if (sizeChangerType != SizeManager.SizeChangerType.LocalOffline)
		{
			if (sizeChangerType - SizeManager.SizeChangerType.LocalOnline <= 1)
			{
				this.lastSizeChanger = this.ControllingChanger(this.targetRig.transform);
				num = this.ScaleFromChanger(this.lastSizeChanger, this.targetRig.transform);
				this.targetRig.scaleFactor = ((num == 1f) ? this.LerpSizeToNormal(this.targetRig.scaleFactor) : num);
			}
		}
		else
		{
			this.lastSizeChanger = this.ControllingChanger(this.mainCameraTransform);
			num = this.ScaleFromChanger(this.lastSizeChanger, this.mainCameraTransform);
			this.targetPlayer.scale = ((num == 1f) ? this.LerpSizeToNormal(this.targetPlayer.scale) : num);
		}
		if (num != this.lastScale)
		{
			for (int i = 0; i < this.lineRenderers.Length; i++)
			{
				this.lineRenderers[i].widthMultiplier = num * this.initLineScalar[i];
			}
		}
		this.lastScale = num;
	}

	// Token: 0x06000AA1 RID: 2721 RVA: 0x00041C68 File Offset: 0x0003FE68
	public SizeChanger ControllingChanger(Transform t)
	{
		for (int i = this.touchingChangers.Count - 1; i >= 0; i--)
		{
			SizeChanger sizeChanger = this.touchingChangers[i];
			if (!(sizeChanger == null) && sizeChanger.gameObject.activeInHierarchy && (sizeChanger.SizeLayerMask & this.currentSizeLayerMaskValue) != 0 && (sizeChanger.myCollider.ClosestPoint(t.position) - t.position).magnitude < this.magnitudeThreshold)
			{
				return sizeChanger;
			}
		}
		return null;
	}

	// Token: 0x06000AA2 RID: 2722 RVA: 0x00041CF0 File Offset: 0x0003FEF0
	public float ScaleFromChanger(SizeChanger sC, Transform t)
	{
		if (sC == null)
		{
			return 1f;
		}
		SizeChanger.ChangerType changerType = sC.myType;
		if (changerType == SizeChanger.ChangerType.Static)
		{
			return sC.minScale;
		}
		if (changerType == SizeChanger.ChangerType.Continuous)
		{
			Vector3 vector = Vector3.Project(t.position - sC.startPos.position, sC.endPos.position - sC.startPos.position);
			return Mathf.Clamp(sC.maxScale - vector.magnitude / (sC.startPos.position - sC.endPos.position).magnitude * (sC.maxScale - sC.minScale), sC.minScale, sC.maxScale);
		}
		return 1f;
	}

	// Token: 0x06000AA3 RID: 2723 RVA: 0x00041DB8 File Offset: 0x0003FFB8
	public float LerpSizeToNormal(float currentSize)
	{
		if (Mathf.Abs(1f - currentSize) < 0.05f)
		{
			return 1f;
		}
		float num = 0.75f;
		float num2 = (1f - currentSize) / num;
		float num3 = 0f;
		num3 += num2 * Time.fixedDeltaTime;
		return Mathf.Lerp(currentSize, 1f, num3);
	}

	// Token: 0x04000D70 RID: 3440
	public SizeChanger lastSizeChanger;

	// Token: 0x04000D71 RID: 3441
	public List<SizeChanger> touchingChangers;

	// Token: 0x04000D72 RID: 3442
	private LineRenderer[] lineRenderers;

	// Token: 0x04000D73 RID: 3443
	private List<float> initLineScalar = new List<float>();

	// Token: 0x04000D74 RID: 3444
	public VRRig targetRig;

	// Token: 0x04000D75 RID: 3445
	public Player targetPlayer;

	// Token: 0x04000D76 RID: 3446
	public float magnitudeThreshold = 0.01f;

	// Token: 0x04000D77 RID: 3447
	public float rate = 650f;

	// Token: 0x04000D78 RID: 3448
	public Transform mainCameraTransform;

	// Token: 0x04000D79 RID: 3449
	public SizeManager.SizeChangerType myType;

	// Token: 0x04000D7A RID: 3450
	public float lastScale;

	// Token: 0x02000444 RID: 1092
	public enum SizeChangerType
	{
		// Token: 0x04001DAB RID: 7595
		LocalOffline,
		// Token: 0x04001DAC RID: 7596
		LocalOnline,
		// Token: 0x04001DAD RID: 7597
		OtherOnline
	}
}
