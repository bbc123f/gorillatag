using System;
using System.Collections.Generic;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200019C RID: 412
public class SizeManager : MonoBehaviour
{
	// Token: 0x1700007D RID: 125
	// (get) Token: 0x06000A9F RID: 2719 RVA: 0x00041A8E File Offset: 0x0003FC8E
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

	// Token: 0x1700007E RID: 126
	// (get) Token: 0x06000AA0 RID: 2720 RVA: 0x00041AC9 File Offset: 0x0003FCC9
	// (set) Token: 0x06000AA1 RID: 2721 RVA: 0x00041B00 File Offset: 0x0003FD00
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

	// Token: 0x06000AA2 RID: 2722 RVA: 0x00041B5A File Offset: 0x0003FD5A
	private void OnDisable()
	{
		this.touchingChangers.Clear();
		this.currentSizeLayerMaskValue = 1;
	}

	// Token: 0x06000AA3 RID: 2723 RVA: 0x00041B70 File Offset: 0x0003FD70
	private void CollectLineRenderers(GameObject obj)
	{
		this.lineRenderers = obj.GetComponentsInChildren<LineRenderer>(true);
		int num = this.lineRenderers.Length;
		foreach (LineRenderer lineRenderer in this.lineRenderers)
		{
			this.initLineScalar.Add(lineRenderer.widthMultiplier);
		}
	}

	// Token: 0x06000AA4 RID: 2724 RVA: 0x00041BC0 File Offset: 0x0003FDC0
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

	// Token: 0x06000AA5 RID: 2725 RVA: 0x00041C98 File Offset: 0x0003FE98
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

	// Token: 0x06000AA6 RID: 2726 RVA: 0x00041DA0 File Offset: 0x0003FFA0
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

	// Token: 0x06000AA7 RID: 2727 RVA: 0x00041E28 File Offset: 0x00040028
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

	// Token: 0x06000AA8 RID: 2728 RVA: 0x00041EF0 File Offset: 0x000400F0
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

	// Token: 0x04000D74 RID: 3444
	public SizeChanger lastSizeChanger;

	// Token: 0x04000D75 RID: 3445
	public List<SizeChanger> touchingChangers;

	// Token: 0x04000D76 RID: 3446
	private LineRenderer[] lineRenderers;

	// Token: 0x04000D77 RID: 3447
	private List<float> initLineScalar = new List<float>();

	// Token: 0x04000D78 RID: 3448
	public VRRig targetRig;

	// Token: 0x04000D79 RID: 3449
	public Player targetPlayer;

	// Token: 0x04000D7A RID: 3450
	public float magnitudeThreshold = 0.01f;

	// Token: 0x04000D7B RID: 3451
	public float rate = 650f;

	// Token: 0x04000D7C RID: 3452
	public Transform mainCameraTransform;

	// Token: 0x04000D7D RID: 3453
	public SizeManager.SizeChangerType myType;

	// Token: 0x04000D7E RID: 3454
	public float lastScale;

	// Token: 0x02000446 RID: 1094
	public enum SizeChangerType
	{
		// Token: 0x04001DB8 RID: 7608
		LocalOffline,
		// Token: 0x04001DB9 RID: 7609
		LocalOnline,
		// Token: 0x04001DBA RID: 7610
		OtherOnline
	}
}
