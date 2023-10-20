using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000239 RID: 569
public class ThrowableSetDressing : TransferrableObject
{
	// Token: 0x170000AA RID: 170
	// (get) Token: 0x06000E16 RID: 3606 RVA: 0x00051EDC File Offset: 0x000500DC
	// (set) Token: 0x06000E17 RID: 3607 RVA: 0x00051EE4 File Offset: 0x000500E4
	public bool inInitialPose { get; private set; } = true;

	// Token: 0x06000E18 RID: 3608 RVA: 0x00051EED File Offset: 0x000500ED
	public override bool ShouldBeKinematic()
	{
		return this.inInitialPose || base.ShouldBeKinematic();
	}

	// Token: 0x06000E19 RID: 3609 RVA: 0x00051EFF File Offset: 0x000500FF
	protected override void Start()
	{
		base.Start();
		this.respawnAtPos = base.transform.position;
		this.respawnAtRot = base.transform.rotation;
		this.currentState = TransferrableObject.PositionState.Dropped;
	}

	// Token: 0x06000E1A RID: 3610 RVA: 0x00051F34 File Offset: 0x00050134
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		base.OnGrab(pointGrabbed, grabbingHand);
		this.inInitialPose = false;
		this.StopRespawnTimer();
	}

	// Token: 0x06000E1B RID: 3611 RVA: 0x00051F4B File Offset: 0x0005014B
	public override void OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		base.OnRelease(zoneReleased, releasingHand);
		this.StartRespawnTimer(-1f);
	}

	// Token: 0x06000E1C RID: 3612 RVA: 0x00051F60 File Offset: 0x00050160
	public override void DropItem()
	{
		base.DropItem();
		this.StartRespawnTimer(-1f);
	}

	// Token: 0x06000E1D RID: 3613 RVA: 0x00051F73 File Offset: 0x00050173
	private void StopRespawnTimer()
	{
		if (this.respawnTimer != null)
		{
			base.StopCoroutine(this.respawnTimer);
			this.respawnTimer = null;
		}
	}

	// Token: 0x06000E1E RID: 3614 RVA: 0x00051F90 File Offset: 0x00050190
	public void SetWillTeleport()
	{
		this.worldShareableInstance.SetWillTeleport();
	}

	// Token: 0x06000E1F RID: 3615 RVA: 0x00051FA0 File Offset: 0x000501A0
	public void StartRespawnTimer(float overrideTimer = -1f)
	{
		float timerDuration = (overrideTimer != -1f) ? overrideTimer : this.respawnTimerDuration;
		this.StopRespawnTimer();
		if (this.respawnTimerDuration != 0f && (base.photonView == null || base.photonView.IsMine))
		{
			this.respawnTimer = base.StartCoroutine(this.RespawnTimerCoroutine(timerDuration));
		}
	}

	// Token: 0x06000E20 RID: 3616 RVA: 0x00052000 File Offset: 0x00050200
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

	// Token: 0x04001125 RID: 4389
	public float respawnTimerDuration;

	// Token: 0x04001127 RID: 4391
	[Tooltip("set this only if this set dressing is using as an ingredient for the magic cauldron - Halloween")]
	public MagicIngredientType IngredientTypeSO;

	// Token: 0x04001128 RID: 4392
	private float _respawnTimestamp;

	// Token: 0x04001129 RID: 4393
	[SerializeField]
	private CapsuleCollider capsuleCollider;

	// Token: 0x0400112A RID: 4394
	private Vector3 respawnAtPos;

	// Token: 0x0400112B RID: 4395
	private Quaternion respawnAtRot;

	// Token: 0x0400112C RID: 4396
	private Coroutine respawnTimer;
}
