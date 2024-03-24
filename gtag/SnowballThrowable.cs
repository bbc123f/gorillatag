using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using GorillaTag;
using GorillaTagScripts;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Serialization;

public class SnowballThrowable : HoldableObject
{
	public event SnowballThrowable.SnowballHitEvent OnSnowballHitPlayer
	{
		[CompilerGenerated]
		add
		{
			SnowballThrowable.SnowballHitEvent snowballHitEvent = this.OnSnowballHitPlayer;
			SnowballThrowable.SnowballHitEvent snowballHitEvent2;
			do
			{
				snowballHitEvent2 = snowballHitEvent;
				SnowballThrowable.SnowballHitEvent snowballHitEvent3 = (SnowballThrowable.SnowballHitEvent)Delegate.Combine(snowballHitEvent2, value);
				snowballHitEvent = Interlocked.CompareExchange<SnowballThrowable.SnowballHitEvent>(ref this.OnSnowballHitPlayer, snowballHitEvent3, snowballHitEvent2);
			}
			while (snowballHitEvent != snowballHitEvent2);
		}
		[CompilerGenerated]
		remove
		{
			SnowballThrowable.SnowballHitEvent snowballHitEvent = this.OnSnowballHitPlayer;
			SnowballThrowable.SnowballHitEvent snowballHitEvent2;
			do
			{
				snowballHitEvent2 = snowballHitEvent;
				SnowballThrowable.SnowballHitEvent snowballHitEvent3 = (SnowballThrowable.SnowballHitEvent)Delegate.Remove(snowballHitEvent2, value);
				snowballHitEvent = Interlocked.CompareExchange<SnowballThrowable.SnowballHitEvent>(ref this.OnSnowballHitPlayer, snowballHitEvent3, snowballHitEvent2);
			}
			while (snowballHitEvent != snowballHitEvent2);
		}
	}

	internal int ProjectileHash
	{
		get
		{
			return PoolUtils.GameObjHashCode(this.randomModelSelection ? this.localModels[this.randModelIndex].projectilePrefab : this.projectilePrefab);
		}
	}

	protected void Awake()
	{
		if (this.awakeHasBeenCalled)
		{
			return;
		}
		this.targetRig = base.GetComponentInParent<VRRig>();
		this.isOfflineRig = this.targetRig != null && this.targetRig.isOfflineVRRig;
		this.awakeHasBeenCalled = true;
		this.renderers = base.GetComponentsInChildren<Renderer>();
		this.matPropBlock = new MaterialPropertyBlock();
		this.randModelIndex = -1;
	}

	public bool IsMine()
	{
		return this.targetRig != null && this.targetRig.isOfflineVRRig;
	}

	public override void OnEnable()
	{
		base.OnEnable();
		if (this.targetRig == null)
		{
			Debug.LogError("SnowballThrowable: targetRig is null! Deactivating.");
			base.gameObject.SetActive(false);
			return;
		}
		if (!this.targetRig.isOfflineVRRig)
		{
			if (this.targetRig.photonView != null && this.targetRig.photonView.IsMine)
			{
				base.gameObject.SetActive(false);
				return;
			}
			Color throwableProjectileColor = this.targetRig.GetThrowableProjectileColor(this.isLeftHanded);
			this.ApplyColor(throwableProjectileColor);
			if (this.randomModelSelection)
			{
				foreach (RandomBucketThrowable randomBucketThrowable in this.localModels)
				{
					randomBucketThrowable.gameObject.SetActive(false);
				}
				this.randModelIndex = this.targetRig.GetRandomThrowableModelIndex();
				this.EnableRandomModel(this.randModelIndex, true);
			}
		}
		this.AnchorToHand();
		this.OnEnableHasBeenCalled = true;
	}

	public void EnableSnowballLocal(bool enable)
	{
		if (!this.awakeHasBeenCalled)
		{
			this.Awake();
		}
		if (!this.OnEnableHasBeenCalled)
		{
			this.OnEnable();
		}
		if (this.isLeftHanded)
		{
			this.targetRig.LeftThrowableProjectileIndex = (enable ? this.throwableMakerIndex : (-1));
		}
		else
		{
			this.targetRig.RightThrowableProjectileIndex = (enable ? this.throwableMakerIndex : (-1));
		}
		base.gameObject.SetActive(enable);
		if (this.randomModelSelection)
		{
			if (enable)
			{
				this.EnableRandomModel(this.GetRandomModelIndex(), true);
			}
			else
			{
				this.EnableRandomModel(this.randModelIndex, false);
			}
			this.targetRig.SetRandomThrowableModelIndex(this.randModelIndex);
		}
		EquipmentInteractor.instance.UpdateHandEquipment(enable ? this : null, this.isLeftHanded);
		if (this.randomizeColor)
		{
			Color color = (enable ? GTColor.RandomHSV(this.randomColorHSVRanges) : Color.white);
			this.targetRig.SetThrowableProjectileColor(this.isLeftHanded, color);
			this.ApplyColor(color);
		}
	}

	private int GetRandomModelIndex()
	{
		if (this.localModels.Count == 0)
		{
			return -1;
		}
		this.randModelIndex = Random.Range(0, this.localModels.Count);
		if ((float)Random.Range(1, 100) >= this.localModels[this.randModelIndex].weightedChance)
		{
			return this.randModelIndex;
		}
		return this.GetRandomModelIndex();
	}

	private void EnableRandomModel(int index, bool enable)
	{
		if (this.randModelIndex >= 0 && this.randModelIndex < this.localModels.Count)
		{
			this.localModels[this.randModelIndex].gameObject.SetActive(enable);
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
		BodyDockPositions myBodyDockPositions = this.targetRig.myBodyDockPositions;
		Transform transform = this.Anchor();
		if (this.isLeftHanded)
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
		if (this.IsMine())
		{
			this.LateUpdateLocal();
		}
		else
		{
			this.LateUpdateReplicated();
		}
		this.LateUpdateShared();
	}

	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		this.LaunchSnowball();
		return true;
	}

	private void LaunchSnowball()
	{
		SlingshotProjectile component = ObjectPools.instance.Instantiate(this.randomModelSelection ? this.localModels[this.randModelIndex].projectilePrefab : this.projectilePrefab).GetComponent<SlingshotProjectile>();
		Transform transform = base.transform;
		Vector3 position = transform.position;
		float x = transform.lossyScale.x;
		Vector3 vector = this.velocityEstimator.linearVelocity;
		Vector3 angularVelocity = this.velocityEstimator.angularVelocity;
		Vector3 handPos = this.velocityEstimator.handPos;
		Vector3 vector2 = Vector3.zero;
		float magnitude = vector.magnitude;
		float magnitude2 = vector2.magnitude;
		if (magnitude > 0.001f)
		{
			float num = Mathf.Clamp(magnitude * this.linSpeedMultiplier, 0f, this.maxLinSpeed);
			vector *= num / magnitude;
		}
		if (magnitude2 > 0.001f)
		{
			float num2 = Mathf.Clamp(magnitude2, 0f, this.maxWristSpeed);
			vector2 *= num2 / magnitude2;
		}
		Vector3 vector3 = Vector3.zero;
		Rigidbody component2 = GorillaTagger.Instance.GetComponent<Rigidbody>();
		if (component2 != null)
		{
			vector3 = component2.velocity;
		}
		Vector3 vector4 = vector + vector2 + vector3;
		Color throwableProjectileColor = this.targetRig.GetThrowableProjectileColor(this.isLeftHanded);
		if (PhotonNetwork.InRoom)
		{
			int num3 = ProjectileTracker.IncrementLocalPlayerProjectileCount();
			component.Launch(base.transform.position, vector4, PhotonNetwork.LocalPlayer, false, false, num3, x, this.randomizeColor, throwableProjectileColor);
			RoomSystem.SendLaunchProjectile(position, vector4, this.isLeftHanded ? RoomSystem.ProjectileSource.LeftHand : RoomSystem.ProjectileSource.RightHand, num3, this.randomizeColor, throwableProjectileColor.r, throwableProjectileColor.g, throwableProjectileColor.b, throwableProjectileColor.a);
		}
		else
		{
			component.Launch(position, vector4, PhotonNetwork.LocalPlayer, false, false, 0, x, this.randomizeColor, throwableProjectileColor);
		}
		component.OnHitPlayer += this.OnProjectileHitPlayer;
		this.launchSoundBankPlayer.Play(null, null);
		this.EnableSnowballLocal(false);
	}

	private void OnProjectileHitPlayer(Player hitPlayer)
	{
		ScienceExperimentManager instance = ScienceExperimentManager.instance;
		if (instance != null && this.projectilePrefab != null && this.projectilePrefab == instance.waterBalloonPrefab)
		{
			instance.OnWaterBalloonHitPlayer(hitPlayer);
		}
	}

	private void ApplyColor(Color newColor)
	{
		foreach (Renderer renderer in this.renderers)
		{
			if (renderer)
			{
				foreach (Material material in renderer.materials)
				{
					if (!(material == null))
					{
						if (material.HasProperty("_BaseColor"))
						{
							material.SetColor("_BaseColor", newColor);
						}
						if (material.HasProperty("_Color"))
						{
							material.SetColor("_Color", newColor);
						}
					}
				}
			}
		}
	}

	public SnowballThrowable()
	{
	}

	// Note: this type is marked as 'beforefieldinit'.
	static SnowballThrowable()
	{
	}

	[GorillaSoundLookup]
	public List<int> matDataIndexes = new List<int> { 32 };

	public GameObject projectilePrefab;

	[FormerlySerializedAs("shouldColorize")]
	public bool randomizeColor;

	public GTColor.HSVRanges randomColorHSVRanges = new GTColor.HSVRanges(0f, 1f, 0.7f, 1f, 1f, 1f);

	public GorillaVelocityEstimator velocityEstimator;

	public SoundBankPlayer launchSoundBankPlayer;

	public float linSpeedMultiplier = 1f;

	public float maxLinSpeed = 12f;

	public float maxWristSpeed = 4f;

	public bool isLeftHanded;

	[CompilerGenerated]
	private SnowballThrowable.SnowballHitEvent OnSnowballHitPlayer;

	[Tooltip("Check this part only if we want to randomize the prefab meshes and projectile")]
	public bool randomModelSelection;

	public List<RandomBucketThrowable> localModels;

	[NonSerialized]
	public int throwableMakerIndex;

	private VRRig targetRig;

	private bool isOfflineRig;

	private bool awakeHasBeenCalled;

	private bool OnEnableHasBeenCalled;

	private MaterialPropertyBlock matPropBlock;

	private static readonly int colorShaderProp = Shader.PropertyToID("_Color");

	private Renderer[] renderers;

	private int randModelIndex;

	public delegate void SnowballHitEvent(Player hitPlayer);
}
