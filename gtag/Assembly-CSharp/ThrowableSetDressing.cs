using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000238 RID: 568
public class ThrowableSetDressing : TransferrableObject
{
	// Token: 0x170000A8 RID: 168
	// (get) Token: 0x06000E0F RID: 3599 RVA: 0x00051B00 File Offset: 0x0004FD00
	// (set) Token: 0x06000E10 RID: 3600 RVA: 0x00051B08 File Offset: 0x0004FD08
	public bool inInitialPose { get; private set; } = true;

	// Token: 0x06000E11 RID: 3601 RVA: 0x00051B11 File Offset: 0x0004FD11
	public override bool ShouldBeKinematic()
	{
		return this.inInitialPose || base.ShouldBeKinematic();
	}

	// Token: 0x06000E12 RID: 3602 RVA: 0x00051B23 File Offset: 0x0004FD23
	protected override void Start()
	{
		base.Start();
		this.respawnAtPos = base.transform.position;
		this.respawnAtRot = base.transform.rotation;
		this.currentState = TransferrableObject.PositionState.Dropped;
	}

	// Token: 0x06000E13 RID: 3603 RVA: 0x00051B58 File Offset: 0x0004FD58
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		base.OnGrab(pointGrabbed, grabbingHand);
		this.inInitialPose = false;
		this.StopRespawnTimer();
	}

	// Token: 0x06000E14 RID: 3604 RVA: 0x00051B6F File Offset: 0x0004FD6F
	public override void OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		base.OnRelease(zoneReleased, releasingHand);
		this.StartRespawnTimer(-1f);
	}

	// Token: 0x06000E15 RID: 3605 RVA: 0x00051B84 File Offset: 0x0004FD84
	public override void DropItem()
	{
		base.DropItem();
		this.StartRespawnTimer(-1f);
	}

	// Token: 0x06000E16 RID: 3606 RVA: 0x00051B97 File Offset: 0x0004FD97
	private void StopRespawnTimer()
	{
		if (this.respawnTimer != null)
		{
			base.StopCoroutine(this.respawnTimer);
			this.respawnTimer = null;
		}
	}

	// Token: 0x06000E17 RID: 3607 RVA: 0x00051BB4 File Offset: 0x0004FDB4
	public void SetWillTeleport()
	{
		this.worldShareableInstance.SetWillTeleport();
	}

	// Token: 0x06000E18 RID: 3608 RVA: 0x00051BC4 File Offset: 0x0004FDC4
	public void StartRespawnTimer(float overrideTimer = -1f)
	{
		float timerDuration = (overrideTimer != -1f) ? overrideTimer : this.respawnTimerDuration;
		this.StopRespawnTimer();
		if (this.respawnTimerDuration != 0f && (base.photonView == null || base.photonView.IsMine))
		{
			this.respawnTimer = base.StartCoroutine(this.RespawnTimerCoroutine(timerDuration));
		}
	}

	// Token: 0x06000E19 RID: 3609 RVA: 0x00051C24 File Offset: 0x0004FE24
	private IEnumerator RespawnTimerCoroutine(float timerDuration)
	{
		yield return new WaitForSeconds(timerDuration);
		if (base.InHand())
		{
			yield break;
		}
		this.SetWillTeleport();
		base.transform.position = this.respawnAtPos;
		base.transform.rotation = this.respawnAtRot;
		this.inInitialPose = true;
		this.rigidbodyInstance.isKinematic = true;
		yield break;
	}

	// Token: 0x0400111F RID: 4383
	public float respawnTimerDuration;

	// Token: 0x04001121 RID: 4385
	[Tooltip("set this only if this set dressing is using as an ingredient for the magic cauldron - Halloween")]
	public MagicIngredientType IngredientTypeSO;

	// Token: 0x04001122 RID: 4386
	private float _respawnTimestamp;

	// Token: 0x04001123 RID: 4387
	[SerializeField]
	private CapsuleCollider capsuleCollider;

	// Token: 0x04001124 RID: 4388
	private Vector3 respawnAtPos;

	// Token: 0x04001125 RID: 4389
	private Quaternion respawnAtRot;

	// Token: 0x04001126 RID: 4390
	private Coroutine respawnTimer;
}
