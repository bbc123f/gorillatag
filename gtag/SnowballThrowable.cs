using System;
using System.Collections.Generic;
using GorillaTag;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

public class SnowballThrowable : HoldableObject
{
	[GorillaSoundLookup]
	public List<int> matDataIndexes = new List<int> { 32 };

	public GameObject projectilePrefab;

	[FormerlySerializedAs("shouldColorize")]
	public bool randomizeColor;

	public GTColor.HSVRanges randomColorHSVRanges = new GTColor.HSVRanges(0f, 1f, 0.7f, 1f, 1f);

	public GorillaVelocityEstimator velocityEstimator;

	public SoundBankPlayer launchSoundBankPlayer;

	public float linSpeedMultiplier = 1f;

	public float maxLinSpeed = 12f;

	public float maxWristSpeed = 4f;

	public bool isLeftHanded;

	[NonSerialized]
	public int throwableMakerIndex;

	private VRRig targetRig;

	private bool isOfflineRig;

	private bool awakeHasBeenCalled;

	private bool OnEnableHasBeenCalled;

	private MaterialPropertyBlock matPropBlock;

	private static readonly int colorShaderProp = Shader.PropertyToID("_Color");

	private Renderer[] renderers;

	protected void Awake()
	{
		if (!awakeHasBeenCalled)
		{
			targetRig = GetComponentInParent<VRRig>();
			isOfflineRig = targetRig != null && targetRig.isOfflineVRRig;
			awakeHasBeenCalled = true;
			renderers = GetComponentsInChildren<Renderer>();
			matPropBlock = new MaterialPropertyBlock();
		}
	}

	public bool IsMine()
	{
		if (targetRig != null)
		{
			return targetRig.isOfflineVRRig;
		}
		return false;
	}

	public override void OnEnable()
	{
		base.OnEnable();
		if (targetRig == null)
		{
			Debug.LogError("SnowballThrowable: targetRig is null! Deactivating.");
			base.gameObject.SetActive(value: false);
			return;
		}
		if (!targetRig.isOfflineVRRig)
		{
			if (targetRig.photonView != null && targetRig.photonView.IsMine)
			{
				base.gameObject.SetActive(value: false);
				return;
			}
			Color throwableProjectileColor = targetRig.GetThrowableProjectileColor(isLeftHanded);
			ApplyColor(throwableProjectileColor);
		}
		AnchorToHand();
		OnEnableHasBeenCalled = true;
	}

	public void EnableSnowballLocal(bool enable)
	{
		if (!awakeHasBeenCalled)
		{
			Awake();
		}
		if (!OnEnableHasBeenCalled)
		{
			OnEnable();
		}
		if (isLeftHanded)
		{
			targetRig.LeftThrowableProjectileIndex = (enable ? throwableMakerIndex : (-1));
		}
		else
		{
			targetRig.RightThrowableProjectileIndex = (enable ? throwableMakerIndex : (-1));
		}
		base.gameObject.SetActive(enable);
		EquipmentInteractor.instance.UpdateHandEquipment(enable ? this : null, isLeftHanded);
		if (randomizeColor)
		{
			Color color = (enable ? GTColor.RandomHSV(randomColorHSVRanges) : Color.white);
			targetRig.SetThrowableProjectileColor(isLeftHanded, color);
			ApplyColor(color);
		}
	}

	protected void LateUpdateLocal()
	{
	}

	protected void LateUpdateReplicated()
	{
	}

	protected void LateUpdateShared()
	{
	}

	private Transform Anchor()
	{
		return base.transform.parent;
	}

	private void AnchorToHand()
	{
		BodyDockPositions myBodyDockPositions = targetRig.myBodyDockPositions;
		Transform transform = Anchor();
		if (isLeftHanded)
		{
			transform.parent = myBodyDockPositions.leftHandTransform;
		}
		else
		{
			transform.parent = myBodyDockPositions.rightHandTransform;
		}
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
	}

	protected void LateUpdate()
	{
		if (IsMine())
		{
			LateUpdateLocal();
		}
		else
		{
			LateUpdateReplicated();
		}
		LateUpdateShared();
	}

	public override void OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		base.OnRelease(zoneReleased, releasingHand);
		if ((!(releasingHand == EquipmentInteractor.instance.rightHand) || !isLeftHanded) && (!(releasingHand == EquipmentInteractor.instance.leftHand) || isLeftHanded))
		{
			LaunchSnowball();
		}
	}

	private void LaunchSnowball()
	{
		GameObject gameObject = ObjectPools.instance.Instantiate(projectilePrefab);
		SlingshotProjectile component = gameObject.GetComponent<SlingshotProjectile>();
		Transform obj = base.transform;
		Vector3 position = obj.position;
		float x = obj.lossyScale.x;
		Vector3 linearVelocity = velocityEstimator.linearVelocity;
		_ = velocityEstimator.angularVelocity;
		_ = velocityEstimator.handPos;
		Vector3 zero = Vector3.zero;
		float magnitude = linearVelocity.magnitude;
		float magnitude2 = zero.magnitude;
		if (magnitude > 0.001f)
		{
			float num = Mathf.Clamp(magnitude * linSpeedMultiplier, 0f, maxLinSpeed);
			linearVelocity *= num / magnitude;
		}
		if (magnitude2 > 0.001f)
		{
			float num2 = Mathf.Clamp(magnitude2, 0f, maxWristSpeed);
			zero *= num2 / magnitude2;
		}
		Vector3 vector = Vector3.zero;
		Rigidbody component2 = GorillaTagger.Instance.GetComponent<Rigidbody>();
		if (component2 != null)
		{
			vector = component2.velocity;
		}
		Vector3 vector2 = linearVelocity + zero + vector;
		Color throwableProjectileColor = targetRig.GetThrowableProjectileColor(isLeftHanded);
		if (GorillaGameManager.instance != null)
		{
			int num3 = GorillaGameManager.instance.IncrementLocalPlayerProjectileCount();
			component.Launch(base.transform.position, vector2, PhotonNetwork.LocalPlayer, blueTeam: false, orangeTeam: false, num3, x, randomizeColor, throwableProjectileColor);
			GorillaGameManager.instance.photonView.RPC("LaunchSlingshotProjectile", RpcTarget.Others, position, vector2, PoolUtils.GameObjHashCode(gameObject), -1, isLeftHanded, num3, randomizeColor, throwableProjectileColor.r, throwableProjectileColor.g, throwableProjectileColor.b, throwableProjectileColor.a);
		}
		else
		{
			component.Launch(position, vector2, PhotonNetwork.LocalPlayer, blueTeam: false, orangeTeam: false, 0, x, randomizeColor, throwableProjectileColor);
		}
		launchSoundBankPlayer.Play();
		EnableSnowballLocal(enable: false);
	}

	private void ApplyColor(Color newColor)
	{
		Renderer[] array = renderers;
		foreach (Renderer renderer in array)
		{
			if ((bool)renderer)
			{
				matPropBlock.SetColor(colorShaderProp, newColor);
				renderer.SetPropertyBlock(matPropBlock);
			}
		}
	}
}
