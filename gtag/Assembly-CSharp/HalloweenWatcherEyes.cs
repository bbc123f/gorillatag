using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using GorillaLocomotion;
using UnityEngine;

public class HalloweenWatcherEyes : MonoBehaviour
{
	private void Start()
	{
		this.playersViewCenterCosAngle = Mathf.Cos(this.playersViewCenterAngle * 0.017453292f);
		this.watchMinCosAngle = Mathf.Cos(this.watchMaxAngle * 0.017453292f);
		base.StartCoroutine(this.CheckIfNearPlayer(Random.Range(0f, this.timeBetweenUpdates)));
		base.enabled = false;
	}

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

	private void LookNormal()
	{
		this.leftEye.transform.localRotation = Quaternion.identity;
		this.rightEye.transform.localRotation = Quaternion.identity;
		this.lerpValue = 0f;
	}

	public HalloweenWatcherEyes()
	{
	}

	public float timeBetweenUpdates = 5f;

	public float watchRange;

	public float watchMaxAngle;

	public float lerpDuration = 1f;

	public float playersViewCenterAngle = 30f;

	public float durationToBeNormalWhenPlayerLooks = 3f;

	public GameObject leftEye;

	public GameObject rightEye;

	private float playersViewCenterCosAngle;

	private float watchMinCosAngle;

	private float pretendingToBeNormalUntilTimestamp;

	private float lerpValue;

	[CompilerGenerated]
	private sealed class <CheckIfNearPlayer>d__13 : IEnumerator<object>, IEnumerator, IDisposable
	{
		[DebuggerHidden]
		public <CheckIfNearPlayer>d__13(int <>1__state)
		{
			this.<>1__state = <>1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		bool IEnumerator.MoveNext()
		{
			int num = this.<>1__state;
			HalloweenWatcherEyes halloweenWatcherEyes = this;
			switch (num)
			{
			case 0:
				this.<>1__state = -1;
				this.<>2__current = new WaitForSeconds(initialSleep);
				this.<>1__state = 1;
				return true;
			case 1:
				this.<>1__state = -1;
				break;
			case 2:
				this.<>1__state = -1;
				break;
			default:
				return false;
			}
			halloweenWatcherEyes.enabled = ((halloweenWatcherEyes.transform.position - Player.Instance.transform.position).sqrMagnitude < halloweenWatcherEyes.watchRange * halloweenWatcherEyes.watchRange);
			if (!halloweenWatcherEyes.enabled)
			{
				halloweenWatcherEyes.LookNormal();
			}
			this.<>2__current = new WaitForSeconds(halloweenWatcherEyes.timeBetweenUpdates);
			this.<>1__state = 2;
			return true;
		}

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		private int <>1__state;

		private object <>2__current;

		public float initialSleep;

		public HalloweenWatcherEyes <>4__this;
	}
}
