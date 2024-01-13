using System.Collections.Generic;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;

public class SizeManager : MonoBehaviour
{
	public enum SizeChangerType
	{
		LocalOffline,
		LocalOnline,
		OtherOnline
	}

	public SizeChanger lastSizeChanger;

	public List<SizeChanger> touchingChangers;

	private LineRenderer[] lineRenderers;

	private List<float> initLineScalar = new List<float>();

	public VRRig targetRig;

	public Player targetPlayer;

	public float magnitudeThreshold = 0.01f;

	public float rate = 650f;

	public Transform mainCameraTransform;

	public SizeChangerType myType;

	public float lastScale;

	public float currentScale
	{
		get
		{
			if (targetRig != null)
			{
				return targetRig.scaleFactor;
			}
			if (targetPlayer != null)
			{
				return targetPlayer.scale;
			}
			return 1f;
		}
	}

	public int currentSizeLayerMaskValue
	{
		get
		{
			if ((bool)targetPlayer)
			{
				return targetPlayer.sizeLayerMask;
			}
			if ((bool)targetRig)
			{
				return targetRig.SizeLayerMask;
			}
			return 1;
		}
		set
		{
			if ((bool)targetPlayer)
			{
				targetPlayer.sizeLayerMask = value;
				if (targetRig != null)
				{
					targetRig.SizeLayerMask = value;
				}
			}
			else if ((bool)targetRig)
			{
				targetRig.SizeLayerMask = value;
			}
		}
	}

	private void OnDisable()
	{
		touchingChangers.Clear();
		currentSizeLayerMaskValue = 1;
	}

	private void CollectLineRenderers(GameObject obj)
	{
		lineRenderers = obj.GetComponentsInChildren<LineRenderer>(includeInactive: true);
		_ = lineRenderers.Length;
		LineRenderer[] array = lineRenderers;
		foreach (LineRenderer lineRenderer in array)
		{
			initLineScalar.Add(lineRenderer.widthMultiplier);
		}
	}

	private void Awake()
	{
		rate = 650f;
		if (targetRig != null)
		{
			CollectLineRenderers(targetRig.gameObject);
		}
		else if (targetPlayer != null)
		{
			CollectLineRenderers(GorillaTagger.Instance.offlineVRRig.gameObject);
		}
		mainCameraTransform = Camera.main.transform;
		if (targetPlayer != null)
		{
			myType = SizeChangerType.LocalOffline;
		}
		else if (targetRig != null && !targetRig.isOfflineVRRig && targetRig.photonView != null && targetRig.photonView.Owner != PhotonNetwork.LocalPlayer)
		{
			myType = SizeChangerType.OtherOnline;
		}
		else
		{
			myType = SizeChangerType.LocalOnline;
		}
	}

	private void FixedUpdate()
	{
		float num = 1f;
		switch (myType)
		{
		case SizeChangerType.LocalOnline:
		case SizeChangerType.OtherOnline:
			lastSizeChanger = ControllingChanger(targetRig.transform);
			num = ScaleFromChanger(lastSizeChanger, targetRig.transform);
			targetRig.scaleFactor = ((num == 1f) ? LerpSizeToNormal(targetRig.scaleFactor) : num);
			break;
		case SizeChangerType.LocalOffline:
			lastSizeChanger = ControllingChanger(mainCameraTransform);
			num = ScaleFromChanger(lastSizeChanger, mainCameraTransform);
			targetPlayer.scale = ((num == 1f) ? LerpSizeToNormal(targetPlayer.scale) : num);
			break;
		}
		if (num != lastScale)
		{
			for (int i = 0; i < lineRenderers.Length; i++)
			{
				lineRenderers[i].widthMultiplier = num * initLineScalar[i];
			}
		}
		lastScale = num;
	}

	public SizeChanger ControllingChanger(Transform t)
	{
		for (int num = touchingChangers.Count - 1; num >= 0; num--)
		{
			SizeChanger sizeChanger = touchingChangers[num];
			if (!(sizeChanger == null) && sizeChanger.gameObject.activeInHierarchy && (sizeChanger.SizeLayerMask & currentSizeLayerMaskValue) != 0 && (sizeChanger.myCollider.ClosestPoint(t.position) - t.position).magnitude < magnitudeThreshold)
			{
				return sizeChanger;
			}
		}
		return null;
	}

	public float ScaleFromChanger(SizeChanger sC, Transform t)
	{
		if (sC == null)
		{
			return 1f;
		}
		switch (sC.myType)
		{
		case SizeChanger.ChangerType.Continuous:
		{
			Vector3 vector = Vector3.Project(t.position - sC.startPos.position, sC.endPos.position - sC.startPos.position);
			return Mathf.Clamp(sC.maxScale - vector.magnitude / (sC.startPos.position - sC.endPos.position).magnitude * (sC.maxScale - sC.minScale), sC.minScale, sC.maxScale);
		}
		case SizeChanger.ChangerType.Static:
			return sC.minScale;
		default:
			return 1f;
		}
	}

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
}
