using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020000E2 RID: 226
public class Slingshot : TransferrableObject
{
	// Token: 0x0600053D RID: 1341 RVA: 0x00021688 File Offset: 0x0001F888
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

	// Token: 0x0600053E RID: 1342 RVA: 0x000216EC File Offset: 0x0001F8EC
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

	// Token: 0x0600053F RID: 1343 RVA: 0x00021761 File Offset: 0x0001F961
	public override void OnDisable()
	{
		this.DestroyDummyProjectile();
		base.OnDisable();
	}

	// Token: 0x06000540 RID: 1344 RVA: 0x00021770 File Offset: 0x0001F970
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

	// Token: 0x06000541 RID: 1345 RVA: 0x0002198A File Offset: 0x0001FB8A
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

	// Token: 0x06000542 RID: 1346 RVA: 0x000219C7 File Offset: 0x0001FBC7
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

	// Token: 0x06000543 RID: 1347 RVA: 0x00021A02 File Offset: 0x0001FC02
	public static bool IsSlingShotEnabled()
	{
		return !(GorillaTagger.Instance == null) && !(GorillaTagger.Instance.offlineVRRig == null) && GorillaTagger.Instance.offlineVRRig.cosmeticSet.HasItemOfCategory(CosmeticsController.CosmeticCategory.Slingshot);
	}

	// Token: 0x06000544 RID: 1348 RVA: 0x00021A3C File Offset: 0x0001FC3C
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

	// Token: 0x06000545 RID: 1349 RVA: 0x00021A8D File Offset: 0x0001FC8D
	private void AttachTrail(int trailHash, GameObject newProjectile, Vector3 location, bool blueTeam, bool orangeTeam)
	{
		SlingshotProjectileTrail component = ObjectPools.instance.Instantiate(trailHash).GetComponent<SlingshotProjectileTrail>();
		newProjectile.transform.position = location;
		component.AttachTrail(newProjectile, blueTeam, orangeTeam);
	}

	// Token: 0x06000546 RID: 1350 RVA: 0x00021AB8 File Offset: 0x0001FCB8
	private void PlayLaunchSfx()
	{
		if (this.shootSfx != null && this.shootSfxClips != null && this.shootSfxClips.Length != 0)
		{
			this.shootSfx.PlayOneShot(this.shootSfxClips[Random.Range(0, this.shootSfxClips.Length)]);
		}
	}

	// Token: 0x06000547 RID: 1351 RVA: 0x00021B04 File Offset: 0x0001FD04
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

	// Token: 0x06000548 RID: 1352 RVA: 0x00021D48 File Offset: 0x0001FF48
	public void LaunchNetworkedProjectile(Vector3 location, Vector3 velocity, int projHash, int trailHash, int projectileCounter, float scale, bool shouldOverrideColor, Color color, PhotonMessageInfo info)
	{
		SlingshotProjectile slingshotProjectile = null;
		try
		{
			slingshotProjectile = ObjectPools.instance.Instantiate(projHash).GetComponent<SlingshotProjectile>();
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
		}
	}

	// Token: 0x06000549 RID: 1353 RVA: 0x00021E40 File Offset: 0x00020040
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

	// Token: 0x0600054A RID: 1354 RVA: 0x00021E7C File Offset: 0x0002007C
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

	// Token: 0x0600054B RID: 1355 RVA: 0x00021F58 File Offset: 0x00020158
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

	// Token: 0x0600054C RID: 1356 RVA: 0x0002203F File Offset: 0x0002023F
	public override void DropItemCleanup()
	{
		base.DropItemCleanup();
		this.currentState = TransferrableObject.PositionState.OnChest;
		this.itemState = TransferrableObject.ItemStates.State0;
	}

	// Token: 0x0600054D RID: 1357 RVA: 0x00022056 File Offset: 0x00020256
	public override bool AutoGrabTrue(bool leftGrabbingHand)
	{
		return true;
	}

	// Token: 0x0600054E RID: 1358 RVA: 0x00022059 File Offset: 0x00020259
	private bool ForLeftHandSlingshot()
	{
		return this.itemState == TransferrableObject.ItemStates.State2 || this.currentState == TransferrableObject.PositionState.InLeftHand;
	}

	// Token: 0x0600054F RID: 1359 RVA: 0x0002206F File Offset: 0x0002026F
	private bool InDrawingState()
	{
		return this.itemState == TransferrableObject.ItemStates.State2 || this.itemState == TransferrableObject.ItemStates.State3;
	}

	// Token: 0x04000629 RID: 1577
	[FormerlySerializedAs("elastic")]
	public LineRenderer elasticLeft;

	// Token: 0x0400062A RID: 1578
	public LineRenderer elasticRight;

	// Token: 0x0400062B RID: 1579
	public Transform leftArm;

	// Token: 0x0400062C RID: 1580
	public Transform rightArm;

	// Token: 0x0400062D RID: 1581
	public Transform center;

	// Token: 0x0400062E RID: 1582
	public Transform centerOrigin;

	// Token: 0x0400062F RID: 1583
	private GameObject dummyProjectile;

	// Token: 0x04000630 RID: 1584
	public GameObject drawingHand;

	// Token: 0x04000631 RID: 1585
	public GameObject projectilePrefab;

	// Token: 0x04000632 RID: 1586
	public GameObject projectileTrail;

	// Token: 0x04000633 RID: 1587
	public InteractionPoint nock;

	// Token: 0x04000634 RID: 1588
	public InteractionPoint grip;

	// Token: 0x04000635 RID: 1589
	public float springConstant;

	// Token: 0x04000636 RID: 1590
	public float maxDraw;

	// Token: 0x04000637 RID: 1591
	public Transform leftHandSnap;

	// Token: 0x04000638 RID: 1592
	public Transform rightHandSnap;

	// Token: 0x04000639 RID: 1593
	public Transform chestSnap;

	// Token: 0x0400063A RID: 1594
	public AudioClip[] shootSfxClips;

	// Token: 0x0400063B RID: 1595
	public AudioSource shootSfx;

	// Token: 0x0400063C RID: 1596
	public bool disableWhenNotInRoom;

	// Token: 0x0400063D RID: 1597
	private bool hasDummyProjectile;

	// Token: 0x0400063E RID: 1598
	private float delayLaunchTime = 0.07f;

	// Token: 0x0400063F RID: 1599
	private float minTimeToLaunch = -1f;

	// Token: 0x04000640 RID: 1600
	private float dummyProjectileColliderRadius;

	// Token: 0x04000641 RID: 1601
	private float dummyProjectileInitialScale;

	// Token: 0x04000642 RID: 1602
	private int projectileCount;

	// Token: 0x04000643 RID: 1603
	private Vector3[] elasticLeftPoints = new Vector3[2];

	// Token: 0x04000644 RID: 1604
	private Vector3[] elasticRightPoints = new Vector3[2];

	// Token: 0x04000645 RID: 1605
	private Dictionary<int, SlingshotProjectile> activeProjectiles = new Dictionary<int, SlingshotProjectile>();

	// Token: 0x020003E9 RID: 1001
	public enum SlingshotState
	{
		// Token: 0x04001C68 RID: 7272
		NoState = 1,
		// Token: 0x04001C69 RID: 7273
		OnChest,
		// Token: 0x04001C6A RID: 7274
		LeftHandDrawing = 4,
		// Token: 0x04001C6B RID: 7275
		RightHandDrawing = 8
	}

	// Token: 0x020003EA RID: 1002
	public enum SlingshotActions
	{
		// Token: 0x04001C6D RID: 7277
		Grab,
		// Token: 0x04001C6E RID: 7278
		Release
	}
}
