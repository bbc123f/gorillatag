using System;
using System.Collections.Generic;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;

public class SizeManager : MonoBehaviour
{
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

	private void OnDisable()
	{
		this.touchingChangers.Clear();
		this.currentSizeLayerMaskValue = 1;
		SizeManagerManager.UnregisterSM(this);
	}

	private void OnEnable()
	{
		SizeManagerManager.RegisterSM(this);
	}

	private void CollectLineRenderers(GameObject obj)
	{
		this.lineRenderers = obj.GetComponentsInChildren<LineRenderer>(true);
		int num = this.lineRenderers.Length;
		foreach (LineRenderer lineRenderer in this.lineRenderers)
		{
			this.initLineScalar.Add(lineRenderer.widthMultiplier);
		}
	}

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
		}
		else if (this.targetRig != null && !this.targetRig.isOfflineVRRig && this.targetRig.photonView != null && this.targetRig.photonView.Owner != PhotonNetwork.LocalPlayer)
		{
			this.myType = SizeManager.SizeChangerType.OtherOnline;
		}
		else
		{
			this.myType = SizeManager.SizeChangerType.LocalOnline;
		}
		SizeManagerManager.RegisterSM(this);
	}

	public void InvokeFixedUpdate()
	{
		float num = 1f;
		SizeManager.SizeChangerType sizeChangerType = this.myType;
		if (sizeChangerType != SizeManager.SizeChangerType.LocalOffline)
		{
			if (sizeChangerType - SizeManager.SizeChangerType.LocalOnline <= 1)
			{
				num = this.ScaleFromChanger(this.ControllingChanger(this.targetRig.transform), this.targetRig.transform, Time.fixedDeltaTime);
				this.targetRig.scaleFactor = ((num == 1f) ? this.SizeOverTime(num, 0.33f, Time.fixedDeltaTime) : num);
			}
		}
		else
		{
			num = this.ScaleFromChanger(this.ControllingChanger(this.targetRig.transform), this.mainCameraTransform, Time.fixedDeltaTime);
			this.targetPlayer.scale = ((num == 1f) ? this.SizeOverTime(num, 0.33f, Time.fixedDeltaTime) : num);
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

	private SizeChanger ControllingChanger(Transform t)
	{
		for (int i = this.touchingChangers.Count - 1; i >= 0; i--)
		{
			SizeChanger sizeChanger = this.touchingChangers[i];
			if (!(sizeChanger == null) && sizeChanger.gameObject.activeInHierarchy && (sizeChanger.SizeLayerMask & this.currentSizeLayerMaskValue) != 0 && (sizeChanger.ClosestPoint(t.position) - t.position).magnitude < this.magnitudeThreshold)
			{
				return sizeChanger;
			}
		}
		return null;
	}

	private float ScaleFromChanger(SizeChanger sC, Transform t, float deltaTime)
	{
		if (sC == null)
		{
			return 1f;
		}
		SizeChanger.ChangerType changerType = sC.MyType;
		if (changerType == SizeChanger.ChangerType.Static)
		{
			return this.SizeOverTime(sC.MinScale, sC.StaticEasing, deltaTime);
		}
		if (changerType == SizeChanger.ChangerType.Continuous)
		{
			Vector3 vector = Vector3.Project(t.position - sC.StartPos.position, sC.EndPos.position - sC.StartPos.position);
			return Mathf.Clamp(sC.MaxScale - vector.magnitude / (sC.StartPos.position - sC.EndPos.position).magnitude * (sC.MaxScale - sC.MinScale), sC.MinScale, sC.MaxScale);
		}
		return 1f;
	}

	private float SizeOverTime(float targetSize, float easing, float deltaTime)
	{
		if (easing <= 0f || Mathf.Abs(this.targetRig.scaleFactor - targetSize) < 0.05f)
		{
			return targetSize;
		}
		return Mathf.MoveTowards(this.targetRig.scaleFactor, targetSize, deltaTime / easing);
	}

	public List<SizeChanger> touchingChangers;

	private LineRenderer[] lineRenderers;

	private List<float> initLineScalar = new List<float>();

	public VRRig targetRig;

	public Player targetPlayer;

	public float magnitudeThreshold = 0.01f;

	public float rate = 650f;

	public Transform mainCameraTransform;

	public SizeManager.SizeChangerType myType;

	public float lastScale;

	private const float returnToNormalEasing = 0.33f;

	public enum SizeChangerType
	{
		LocalOffline,
		LocalOnline,
		OtherOnline
	}
}
