using System;
using GorillaLocomotion;
using UnityEngine;

public class ElderGorilla : MonoBehaviour
{
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

	private void CheckMicVolume()
	{
		float currentPeakAmp = GorillaTagger.Instance.myRecorder.LevelMeter.CurrentPeakAmp;
	}

	public ElderGorilla()
	{
	}

	private const float MAX_HAND_DIST = 1f;

	private const float COOLDOWN_HAND_DIST = 1f;

	private const float VALID_HAND_DIST = 0.75f;

	private const float TIME_VALID_HEAD_HEIGHT = 1f;

	private Transform tHMD;

	private Transform tLeftHand;

	private Transform tRightHand;

	private int countValidArmDists;

	private float timeLastValidArmDist;

	private bool trackingHeadHeight;

	private float trackedHeadHeight;

	private float timerTrackedHeadHeight;

	private float savedHeadHeight = 1.5f;
}
