using System;
using System.Collections;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x020001C7 RID: 455
public class HalloweenWatcherEyes : MonoBehaviour
{
	// Token: 0x06000B8C RID: 2956 RVA: 0x00047250 File Offset: 0x00045450
	private void Start()
	{
		this.playersViewCenterCosAngle = Mathf.Cos(this.playersViewCenterAngle * 0.017453292f);
		this.watchMinCosAngle = Mathf.Cos(this.watchMaxAngle * 0.017453292f);
		base.StartCoroutine(this.CheckIfNearPlayer(Random.Range(0f, this.timeBetweenUpdates)));
		base.enabled = false;
	}

	// Token: 0x06000B8D RID: 2957 RVA: 0x000472AF File Offset: 0x000454AF
	private IEnumerator CheckIfNearPlayer(float initialSleep)
	{
		yield return new WaitForSeconds(initialSleep);
		for (;;)
		{
			base.enabled = ((base.transform.position - Player.Instance.transform.position).sqrMagnitude < this.watchRange * this.watchRange);
			if (!base.enabled)
			{
				this.LookNormal();
			}
			yield return new WaitForSeconds(this.timeBetweenUpdates);
		}
		yield break;
	}

	// Token: 0x06000B8E RID: 2958 RVA: 0x000472C8 File Offset: 0x000454C8
	private void Update()
	{
		Vector3 normalized = (Player.Instance.headCollider.transform.position - base.transform.position).normalized;
		if (Vector3.Dot(Player.Instance.headCollider.transform.forward, -normalized) > this.playersViewCenterCosAngle)
		{
			this.LookNormal();
			this.pretendingToBeNormalUntilTimestamp = Time.time + this.durationToBeNormalWhenPlayerLooks;
		}
		if (this.pretendingToBeNormalUntilTimestamp > Time.time)
		{
			return;
		}
		if (Vector3.Dot(base.transform.forward, normalized) < this.watchMinCosAngle)
		{
			this.LookNormal();
			return;
		}
		Quaternion b = Quaternion.LookRotation(normalized, base.transform.up);
		Quaternion rotation = Quaternion.Lerp(base.transform.rotation, b, this.lerpValue);
		this.leftEye.transform.rotation = rotation;
		this.rightEye.transform.rotation = rotation;
		if (this.lerpDuration > 0f)
		{
			this.lerpValue = Mathf.MoveTowards(this.lerpValue, 1f, Time.deltaTime / this.lerpDuration);
			return;
		}
		this.lerpValue = 1f;
	}

	// Token: 0x06000B8F RID: 2959 RVA: 0x000473F6 File Offset: 0x000455F6
	private void LookNormal()
	{
		this.leftEye.transform.localRotation = Quaternion.identity;
		this.rightEye.transform.localRotation = Quaternion.identity;
		this.lerpValue = 0f;
	}

	// Token: 0x04000F18 RID: 3864
	public float timeBetweenUpdates = 5f;

	// Token: 0x04000F19 RID: 3865
	public float watchRange;

	// Token: 0x04000F1A RID: 3866
	public float watchMaxAngle;

	// Token: 0x04000F1B RID: 3867
	public float lerpDuration = 1f;

	// Token: 0x04000F1C RID: 3868
	public float playersViewCenterAngle = 30f;

	// Token: 0x04000F1D RID: 3869
	public float durationToBeNormalWhenPlayerLooks = 3f;

	// Token: 0x04000F1E RID: 3870
	public GameObject leftEye;

	// Token: 0x04000F1F RID: 3871
	public GameObject rightEye;

	// Token: 0x04000F20 RID: 3872
	private float playersViewCenterCosAngle;

	// Token: 0x04000F21 RID: 3873
	private float watchMinCosAngle;

	// Token: 0x04000F22 RID: 3874
	private float pretendingToBeNormalUntilTimestamp;

	// Token: 0x04000F23 RID: 3875
	private float lerpValue;
}
