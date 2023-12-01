using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Serialization;

public class Slingshot : TransferrableObject
{
	private void DestroyDummyProjectile()
	{
		if (this.hasDummyProjectile)
		{
			this.dummyProjectile.transform.localScale = Vector3.one * this.dummyProjectileInitialScale;
			this.dummyProjectile.GetComponent<SphereCollider>().enabled = true;
			ObjectPools.instance.Destroy(this.dummyProjectile);
			this.dummyProjectile = null;
			this.hasDummyProjectile = false;
		}
	}

	public override void OnEnable()
	{
		this.currentState = TransferrableObject.PositionState.OnChest;
		this.itemState = TransferrableObject.ItemStates.State0;
		this.elasticLeft.positionCount = 2;
		this.elasticRight.positionCount = 2;
		this.dummyProjectile = null;
		if (this.myOnlineRig != null)
		{
			this.myOnlineRig.slingshot = this;
		}
		if (this.myRig != null)
		{
			this.myRig.slingshot = this;
		}
		base.OnEnable();
	}

	public override void OnDisable()
	{
		this.DestroyDummyProjectile();
		base.OnDisable();
	}

	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		float num = Mathf.Abs(base.transform.lossyScale.x);
		Vector3 vector;
		if (this.InDrawingState())
		{
			if (!this.hasDummyProjectile)
			{
				this.dummyProjectile = ObjectPools.instance.Instantiate(this.projectilePrefab);
				this.hasDummyProjectile = true;
				SphereCollider component = this.dummyProjectile.GetComponent<SphereCollider>();
				component.enabled = false;
				this.dummyProjectileColliderRadius = component.radius;
				this.dummyProjectileInitialScale = this.dummyProjectile.transform.localScale.x;
				bool blueTeam;
				bool orangeTeam;
				this.GetIsOnTeams(out blueTeam, out orangeTeam);
				this.dummyProjectile.GetComponent<SlingshotProjectile>().ApplyTeamModelAndColor(blueTeam, orangeTeam, false, default(Color));
			}
			float d = this.dummyProjectileInitialScale * num;
			this.dummyProjectile.transform.localScale = Vector3.one * d;
			Vector3 position = this.drawingHand.transform.position;
			Vector3 position2 = this.centerOrigin.position;
			Vector3 normalized = (position2 - position).normalized;
			float d2 = (EquipmentInteractor.instance.grabRadius - this.dummyProjectileColliderRadius) * num;
			vector = position + normalized * d2;
			this.dummyProjectile.transform.position = vector;
			this.dummyProjectile.transform.rotation = Quaternion.LookRotation(position2 - vector, Vector3.up);
		}
		else
		{
			this.DestroyDummyProjectile();
			vector = this.centerOrigin.position;
		}
		this.center.position = vector;
		this.elasticLeftPoints[0] = this.leftArm.position;
		this.elasticLeftPoints[1] = (this.elasticRightPoints[0] = vector);
		this.elasticRightPoints[1] = this.rightArm.position;
		this.elasticLeft.SetPositions(this.elasticLeftPoints);
		this.elasticRight.SetPositions(this.elasticRightPoints);
		if (!PhotonNetwork.InRoom && this.disableWhenNotInRoom)
		{
			base.gameObject.SetActive(false);
		}
	}

	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		if (this.InDrawingState())
		{
			if (this.ForLeftHandSlingshot())
			{
				this.drawingHand = EquipmentInteractor.instance.rightHand;
				return;
			}
			this.drawingHand = EquipmentInteractor.instance.leftHand;
		}
	}

	protected override void LateUpdateReplicated()
	{
		base.LateUpdateReplicated();
		if (this.InDrawingState())
		{
			if (this.ForLeftHandSlingshot())
			{
				this.drawingHand = this.rightHandSnap.gameObject;
				return;
			}
			this.drawingHand = this.leftHandSnap.gameObject;
		}
	}

	public static bool IsSlingShotEnabled()
	{
		return !(GorillaTagger.Instance == null) && !(GorillaTagger.Instance.offlineVRRig == null) && GorillaTagger.Instance.offlineVRRig.cosmeticSet.HasItemOfCategory(CosmeticsController.CosmeticCategory.Slingshot);
	}

	private void GetIsOnTeams(out bool blueTeam, out bool orangeTeam)
	{
		Photon.Realtime.Player player = base.OwningPlayer();
		blueTeam = false;
		orangeTeam = false;
		if (GorillaGameManager.instance != null)
		{
			GorillaBattleManager component = GorillaGameManager.instance.GetComponent<GorillaBattleManager>();
			if (component != null)
			{
				blueTeam = component.OnBlueTeam(player);
				orangeTeam = component.OnRedTeam(player);
			}
		}
	}

	private void AttachTrail(int trailHash, GameObject newProjectile, Vector3 location, bool blueTeam, bool orangeTeam)
	{
		GameObject gameObject = ObjectPools.instance.Instantiate(trailHash);
		SlingshotProjectileTrail component = gameObject.GetComponent<SlingshotProjectileTrail>();
		if (component.IsNull())
		{
			ObjectPools.instance.Destroy(gameObject);
		}
		newProjectile.transform.position = location;
		component.AttachTrail(newProjectile, blueTeam, orangeTeam);
	}

	private void PlayLaunchSfx()
	{
		if (this.shootSfx != null && this.shootSfxClips != null && this.shootSfxClips.Length != 0)
		{
			this.shootSfx.PlayOneShot(this.shootSfxClips[Random.Range(0, this.shootSfxClips.Length)]);
		}
	}

	private void LaunchProjectile()
	{
		int num = PoolUtils.GameObjHashCode(this.projectilePrefab);
		int num2 = PoolUtils.GameObjHashCode(this.projectileTrail);
		GameObject gameObject = ObjectPools.instance.Instantiate(num);
		float num3 = Mathf.Abs(base.transform.lossyScale.x);
		gameObject.transform.localScale = Vector3.one * num3;
		Vector3 position = this.dummyProjectile.transform.position;
		Vector3 a = this.centerOrigin.position - this.center.position;
		a /= num3;
		Vector3 currentVelocity = GorillaLocomotion.Player.Instance.currentVelocity;
		Vector3 vector = Mathf.Min(this.springConstant * this.maxDraw, a.magnitude * this.springConstant) * a.normalized;
		vector *= num3;
		vector += currentVelocity;
		bool blueTeam;
		bool orangeTeam;
		this.GetIsOnTeams(out blueTeam, out orangeTeam);
		this.AttachTrail(num2, gameObject, position, blueTeam, orangeTeam);
		SlingshotProjectile component = gameObject.GetComponent<SlingshotProjectile>();
		if (GorillaGameManager.instance != null)
		{
			int num4 = GorillaGameManager.instance.IncrementLocalPlayerProjectileCount();
			if (this.activeProjectiles.ContainsKey(num4))
			{
				this.activeProjectiles.Remove(num4);
			}
			this.activeProjectiles.Add(num4, component);
			component.Launch(position, vector, PhotonNetwork.LocalPlayer, blueTeam, orangeTeam, num4, num3, false, default(Color));
			bool flag = this.currentState == TransferrableObject.PositionState.InLeftHand;
			GorillaGameManager.instance.photonView.RPC("LaunchSlingshotProjectile", RpcTarget.Others, new object[]
			{
				position,
				vector,
				num,
				num2,
				flag,
				num4,
				false,
				1f,
				1f,
				1f,
				1f
			});
			this.PlayLaunchSfx();
			return;
		}
		component.Launch(position, vector, PhotonNetwork.LocalPlayer, blueTeam, orangeTeam, 0, num3, false, default(Color));
		this.PlayLaunchSfx();
	}

	public void LaunchNetworkedProjectile(Vector3 location, Vector3 velocity, int projHash, int trailHash, int projectileCounter, float scale, bool shouldOverrideColor, Color color, PhotonMessageInfo info)
	{
		GameObject gameObject = null;
		SlingshotProjectile slingshotProjectile = null;
		try
		{
			gameObject = ObjectPools.instance.Instantiate(projHash);
			slingshotProjectile = gameObject.GetComponent<SlingshotProjectile>();
			bool blueTeam;
			bool orangeTeam;
			this.GetIsOnTeams(out blueTeam, out orangeTeam);
			if (trailHash != -1)
			{
				this.AttachTrail(trailHash, slingshotProjectile.gameObject, location, blueTeam, orangeTeam);
			}
			if (this.activeProjectiles.ContainsKey(projectileCounter))
			{
				this.activeProjectiles.Remove(projectileCounter);
			}
			this.activeProjectiles.Add(projectileCounter, slingshotProjectile);
			slingshotProjectile.Launch(location, velocity, info.Sender, blueTeam, orangeTeam, projectileCounter, scale, shouldOverrideColor, color);
			this.PlayLaunchSfx();
		}
		catch
		{
			GorillaNot.instance.SendReport("projectile error", info.Sender.UserId, info.Sender.NickName);
			if (slingshotProjectile != null && slingshotProjectile)
			{
				slingshotProjectile.transform.position = Vector3.zero;
				this.activeProjectiles.Remove(projectileCounter);
				slingshotProjectile.Deactivate();
			}
			else if (gameObject.IsNotNull())
			{
				ObjectPools.instance.Destroy(gameObject);
			}
		}
	}

	public void DestroyProjectile(int projectileCount, Vector3 worldPosition)
	{
		SlingshotProjectile slingshotProjectile;
		if (this.activeProjectiles.TryGetValue(projectileCount, out slingshotProjectile))
		{
			slingshotProjectile.transform.position = worldPosition;
			this.activeProjectiles.Remove(projectileCount);
			slingshotProjectile.Deactivate();
		}
	}

	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		if (!this.IsMyItem())
		{
			return;
		}
		bool flag = pointGrabbed == this.nock;
		if (flag && !base.InHand())
		{
			return;
		}
		base.OnGrab(pointGrabbed, grabbingHand);
		if (this.InDrawingState() || base.OnChest())
		{
			return;
		}
		if (flag)
		{
			if (grabbingHand == EquipmentInteractor.instance.leftHand)
			{
				EquipmentInteractor.instance.disableLeftGrab = true;
			}
			else
			{
				EquipmentInteractor.instance.disableRightGrab = true;
			}
			if (this.ForLeftHandSlingshot())
			{
				this.itemState = TransferrableObject.ItemStates.State2;
			}
			else
			{
				this.itemState = TransferrableObject.ItemStates.State3;
			}
			this.minTimeToLaunch = Time.time + this.delayLaunchTime;
			GorillaTagger.Instance.StartVibration(!this.ForLeftHandSlingshot(), GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration * 1.5f);
		}
	}

	public override void OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		base.OnRelease(zoneReleased, releasingHand);
		if (!this.IsMyItem())
		{
			return;
		}
		if (this.InDrawingState() && releasingHand == this.drawingHand)
		{
			if (releasingHand == EquipmentInteractor.instance.leftHand)
			{
				EquipmentInteractor.instance.disableLeftGrab = false;
			}
			else
			{
				EquipmentInteractor.instance.disableRightGrab = false;
			}
			if (this.ForLeftHandSlingshot())
			{
				this.currentState = TransferrableObject.PositionState.InLeftHand;
			}
			else
			{
				this.currentState = TransferrableObject.PositionState.InRightHand;
			}
			this.itemState = TransferrableObject.ItemStates.State0;
			GorillaTagger.Instance.StartVibration(this.ForLeftHandSlingshot(), GorillaTagger.Instance.tapHapticStrength * 2f, GorillaTagger.Instance.tapHapticDuration * 1.5f);
			if (Time.time > this.minTimeToLaunch)
			{
				this.LaunchProjectile();
				return;
			}
		}
		else
		{
			EquipmentInteractor.instance.disableLeftGrab = false;
			EquipmentInteractor.instance.disableRightGrab = false;
		}
	}

	public override void DropItemCleanup()
	{
		base.DropItemCleanup();
		this.currentState = TransferrableObject.PositionState.OnChest;
		this.itemState = TransferrableObject.ItemStates.State0;
	}

	public override bool AutoGrabTrue(bool leftGrabbingHand)
	{
		return true;
	}

	private bool ForLeftHandSlingshot()
	{
		return this.itemState == TransferrableObject.ItemStates.State2 || this.currentState == TransferrableObject.PositionState.InLeftHand;
	}

	private bool InDrawingState()
	{
		return this.itemState == TransferrableObject.ItemStates.State2 || this.itemState == TransferrableObject.ItemStates.State3;
	}

	[FormerlySerializedAs("elastic")]
	public LineRenderer elasticLeft;

	public LineRenderer elasticRight;

	public Transform leftArm;

	public Transform rightArm;

	public Transform center;

	public Transform centerOrigin;

	private GameObject dummyProjectile;

	public GameObject drawingHand;

	public GameObject projectilePrefab;

	public GameObject projectileTrail;

	public InteractionPoint nock;

	public InteractionPoint grip;

	public float springConstant;

	public float maxDraw;

	public Transform leftHandSnap;

	public Transform rightHandSnap;

	public Transform chestSnap;

	public AudioClip[] shootSfxClips;

	public AudioSource shootSfx;

	public bool disableWhenNotInRoom;

	private bool hasDummyProjectile;

	private float delayLaunchTime = 0.07f;

	private float minTimeToLaunch = -1f;

	private float dummyProjectileColliderRadius;

	private float dummyProjectileInitialScale;

	private int projectileCount;

	private Vector3[] elasticLeftPoints = new Vector3[2];

	private Vector3[] elasticRightPoints = new Vector3[2];

	private Dictionary<int, SlingshotProjectile> activeProjectiles = new Dictionary<int, SlingshotProjectile>();

	public enum SlingshotState
	{
		NoState = 1,
		OnChest,
		LeftHandDrawing = 4,
		RightHandDrawing = 8
	}

	public enum SlingshotActions
	{
		Grab,
		Release
	}
}
