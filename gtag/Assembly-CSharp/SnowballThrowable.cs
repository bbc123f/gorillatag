using System;
using System.Collections.Generic;
using GorillaTag;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000031 RID: 49
public class SnowballThrowable : HoldableObject
{
	// Token: 0x14000001 RID: 1
	// (add) Token: 0x06000113 RID: 275 RVA: 0x00009E9C File Offset: 0x0000809C
	// (remove) Token: 0x06000114 RID: 276 RVA: 0x00009ED4 File Offset: 0x000080D4
	public event SnowballThrowable.SnowballHitEvent OnSnowballHitPlayer;

	// Token: 0x06000115 RID: 277 RVA: 0x00009F0C File Offset: 0x0000810C
	protected void Awake()
	{
		if (this.awakeHasBeenCalled)
		{
			return;
		}
		this.targetRig = base.GetComponentInParent<VRRig>();
		this.isOfflineRig = (this.targetRig != null && this.targetRig.isOfflineVRRig);
		this.awakeHasBeenCalled = true;
		this.renderers = base.GetComponentsInChildren<Renderer>();
		this.matPropBlock = new MaterialPropertyBlock();
	}

	// Token: 0x06000116 RID: 278 RVA: 0x00009F6E File Offset: 0x0000816E
	public bool IsMine()
	{
		return this.targetRig != null && this.targetRig.isOfflineVRRig;
	}

	// Token: 0x06000117 RID: 279 RVA: 0x00009F8C File Offset: 0x0000818C
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
		}
		this.AnchorToHand();
		this.OnEnableHasBeenCalled = true;
	}

	// Token: 0x06000118 RID: 280 RVA: 0x0000A02C File Offset: 0x0000822C
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
			this.targetRig.LeftThrowableProjectileIndex = (enable ? this.throwableMakerIndex : -1);
		}
		else
		{
			this.targetRig.RightThrowableProjectileIndex = (enable ? this.throwableMakerIndex : -1);
		}
		base.gameObject.SetActive(enable);
		EquipmentInteractor.instance.UpdateHandEquipment(enable ? this : null, this.isLeftHanded);
		if (this.randomizeColor)
		{
			Color color = enable ? GTColor.RandomHSV(this.randomColorHSVRanges) : Color.white;
			this.targetRig.SetThrowableProjectileColor(this.isLeftHanded, color);
			this.ApplyColor(color);
		}
	}

	// Token: 0x06000119 RID: 281 RVA: 0x0000A0E9 File Offset: 0x000082E9
	protected void LateUpdateLocal()
	{
	}

	// Token: 0x0600011A RID: 282 RVA: 0x0000A0EB File Offset: 0x000082EB
	protected void LateUpdateReplicated()
	{
	}

	// Token: 0x0600011B RID: 283 RVA: 0x0000A0ED File Offset: 0x000082ED
	protected void LateUpdateShared()
	{
	}

	// Token: 0x0600011C RID: 284 RVA: 0x0000A0EF File Offset: 0x000082EF
	private Transform Anchor()
	{
		return base.transform.parent;
	}

	// Token: 0x0600011D RID: 285 RVA: 0x0000A0FC File Offset: 0x000082FC
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

	// Token: 0x0600011E RID: 286 RVA: 0x0000A154 File Offset: 0x00008354
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

	// Token: 0x0600011F RID: 287 RVA: 0x0000A174 File Offset: 0x00008374
	public override void OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		base.OnRelease(zoneReleased, releasingHand);
		if (releasingHand == EquipmentInteractor.instance.rightHand && this.isLeftHanded)
		{
			return;
		}
		if (releasingHand == EquipmentInteractor.instance.leftHand && !this.isLeftHanded)
		{
			return;
		}
		this.LaunchSnowball();
	}

	// Token: 0x06000120 RID: 288 RVA: 0x0000A1CC File Offset: 0x000083CC
	private void LaunchSnowball()
	{
		GameObject gameObject = ObjectPools.instance.Instantiate(this.projectilePrefab);
		SlingshotProjectile component = gameObject.GetComponent<SlingshotProjectile>();
		Transform transform = base.transform;
		Vector3 position = transform.position;
		float x = transform.lossyScale.x;
		Vector3 a = this.velocityEstimator.linearVelocity;
		Vector3 angularVelocity = this.velocityEstimator.angularVelocity;
		Vector3 handPos = this.velocityEstimator.handPos;
		Vector3 vector = Vector3.zero;
		float magnitude = a.magnitude;
		float magnitude2 = vector.magnitude;
		if (magnitude > 0.001f)
		{
			float num = Mathf.Clamp(magnitude * this.linSpeedMultiplier, 0f, this.maxLinSpeed);
			a *= num / magnitude;
		}
		if (magnitude2 > 0.001f)
		{
			float num2 = Mathf.Clamp(magnitude2, 0f, this.maxWristSpeed);
			vector *= num2 / magnitude2;
		}
		Vector3 b = Vector3.zero;
		Rigidbody component2 = GorillaTagger.Instance.GetComponent<Rigidbody>();
		if (component2 != null)
		{
			b = component2.velocity;
		}
		Vector3 vector2 = a + vector + b;
		Color throwableProjectileColor = this.targetRig.GetThrowableProjectileColor(this.isLeftHanded);
		if (GorillaGameManager.instance != null)
		{
			int num3 = GorillaGameManager.instance.IncrementLocalPlayerProjectileCount();
			component.Launch(base.transform.position, vector2, PhotonNetwork.LocalPlayer, false, false, num3, x, this.randomizeColor, throwableProjectileColor);
			GorillaGameManager.instance.photonView.RPC("LaunchSlingshotProjectile", RpcTarget.Others, new object[]
			{
				position,
				vector2,
				PoolUtils.GameObjHashCode(gameObject),
				-1,
				this.isLeftHanded,
				num3,
				this.randomizeColor,
				throwableProjectileColor.r,
				throwableProjectileColor.g,
				throwableProjectileColor.b,
				throwableProjectileColor.a
			});
		}
		else
		{
			component.Launch(position, vector2, PhotonNetwork.LocalPlayer, false, false, 0, x, this.randomizeColor, throwableProjectileColor);
		}
		component.OnHitPlayer += this.OnProjectileHitPlayer;
		this.launchSoundBankPlayer.Play(null, null);
		this.EnableSnowballLocal(false);
	}

	// Token: 0x06000121 RID: 289 RVA: 0x0000A434 File Offset: 0x00008634
	private void OnProjectileHitPlayer(Player hitPlayer)
	{
		RisingLavaManager instance = RisingLavaManager.instance;
		if (instance != null && this.projectilePrefab != null && this.projectilePrefab == instance.waterBalloonPrefab)
		{
			instance.OnWaterBalloonHitPlayer(hitPlayer);
		}
	}

	// Token: 0x06000122 RID: 290 RVA: 0x0000A47C File Offset: 0x0000867C
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

	// Token: 0x04000176 RID: 374
	[GorillaSoundLookup]
	public List<int> matDataIndexes = new List<int>
	{
		32
	};

	// Token: 0x04000177 RID: 375
	public GameObject projectilePrefab;

	// Token: 0x04000178 RID: 376
	[FormerlySerializedAs("shouldColorize")]
	public bool randomizeColor;

	// Token: 0x04000179 RID: 377
	public GTColor.HSVRanges randomColorHSVRanges = new GTColor.HSVRanges(0f, 1f, 0.7f, 1f, 1f, 1f);

	// Token: 0x0400017A RID: 378
	public GorillaVelocityEstimator velocityEstimator;

	// Token: 0x0400017B RID: 379
	public SoundBankPlayer launchSoundBankPlayer;

	// Token: 0x0400017C RID: 380
	public float linSpeedMultiplier = 1f;

	// Token: 0x0400017D RID: 381
	public float maxLinSpeed = 12f;

	// Token: 0x0400017E RID: 382
	public float maxWristSpeed = 4f;

	// Token: 0x0400017F RID: 383
	public bool isLeftHanded;

	// Token: 0x04000181 RID: 385
	[NonSerialized]
	public int throwableMakerIndex;

	// Token: 0x04000182 RID: 386
	private VRRig targetRig;

	// Token: 0x04000183 RID: 387
	private bool isOfflineRig;

	// Token: 0x04000184 RID: 388
	private bool awakeHasBeenCalled;

	// Token: 0x04000185 RID: 389
	private bool OnEnableHasBeenCalled;

	// Token: 0x04000186 RID: 390
	private MaterialPropertyBlock matPropBlock;

	// Token: 0x04000187 RID: 391
	private static readonly int colorShaderProp = Shader.PropertyToID("_Color");

	// Token: 0x04000188 RID: 392
	private Renderer[] renderers;

	// Token: 0x0200038F RID: 911
	// (Invoke) Token: 0x06001AC4 RID: 6852
	public delegate void SnowballHitEvent(Player hitPlayer);
}
