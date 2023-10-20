using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x0200015A RID: 346
public class ElderGorilla : MonoBehaviour
{
	// Token: 0x0600088A RID: 2186 RVA: 0x00034B68 File Offset: 0x00032D68
	private void Update()
	{
		if (Player.Instance == null)
		{
			return;
		}
		if (Player.Instance.inOverlay || !Player.Instance.isUserPresent)
		{
			return;
		}
		this.tHMD = Player.Instance.headCollider.transform;
		this.tLeftHand = Player.Instance.leftControllerTransform;
		this.tRightHand = Player.Instance.rightControllerTransform;
		if (Time.time - this.timeLastValidArmDist > 1f)
		{
			this.CheckHandDistance(this.tLeftHand);
			this.CheckHandDistance(this.tRightHand);
		}
		this.CheckHeight();
		this.CheckMicVolume();
	}

	// Token: 0x0600088B RID: 2187 RVA: 0x00034C08 File Offset: 0x00032E08
	private void CheckHandDistance(Transform hand)
	{
		float num = Vector3.Distance(hand.localPosition, this.tHMD.localPosition);
		if (num >= 1f)
		{
			return;
		}
		if (num >= 0.75f)
		{
			this.countValidArmDists++;
			this.timeLastValidArmDist = Time.time;
		}
	}

	// Token: 0x0600088C RID: 2188 RVA: 0x00034C58 File Offset: 0x00032E58
	private void CheckHeight()
	{
		float y = this.tHMD.localPosition.y;
		if (!this.trackingHeadHeight)
		{
			this.trackedHeadHeight = y - 0.05f;
			this.timerTrackedHeadHeight = 0f;
		}
		else if (this.trackedHeadHeight < y)
		{
			this.trackingHeadHeight = false;
		}
		if (this.trackingHeadHeight)
		{
			if (this.timerTrackedHeadHeight >= 1f)
			{
				this.savedHeadHeight = y;
				this.trackingHeadHeight = false;
				return;
			}
			this.timerTrackedHeadHeight += Time.deltaTime;
		}
	}

	// Token: 0x0600088D RID: 2189 RVA: 0x00034CDE File Offset: 0x00032EDE
	private void CheckMicVolume()
	{
		float currentPeakAmp = GorillaTagger.Instance.myRecorder.LevelMeter.CurrentPeakAmp;
	}

	// Token: 0x04000AB7 RID: 2743
	private const float MAX_HAND_DIST = 1f;

	// Token: 0x04000AB8 RID: 2744
	private const float COOLDOWN_HAND_DIST = 1f;

	// Token: 0x04000AB9 RID: 2745
	private const float VALID_HAND_DIST = 0.75f;

	// Token: 0x04000ABA RID: 2746
	private const float TIME_VALID_HEAD_HEIGHT = 1f;

	// Token: 0x04000ABB RID: 2747
	private Transform tHMD;

	// Token: 0x04000ABC RID: 2748
	private Transform tLeftHand;

	// Token: 0x04000ABD RID: 2749
	private Transform tRightHand;

	// Token: 0x04000ABE RID: 2750
	private int countValidArmDists;

	// Token: 0x04000ABF RID: 2751
	private float timeLastValidArmDist;

	// Token: 0x04000AC0 RID: 2752
	private bool trackingHeadHeight;

	// Token: 0x04000AC1 RID: 2753
	private float trackedHeadHeight;

	// Token: 0x04000AC2 RID: 2754
	private float timerTrackedHeadHeight;

	// Token: 0x04000AC3 RID: 2755
	private float savedHeadHeight = 1.5f;
}
