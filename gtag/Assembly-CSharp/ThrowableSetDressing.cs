using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ThrowableSetDressing : TransferrableObject
{
	public bool inInitialPose
	{
		[CompilerGenerated]
		get
		{
			return this.<inInitialPose>k__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			this.<inInitialPose>k__BackingField = value;
		}
	} = true;

	public override bool ShouldBeKinematic()
	{
		return this.inInitialPose || base.ShouldBeKinematic();
	}

	protected override void Start()
	{
		base.Start();
		this.respawnAtPos = base.transform.position;
		this.respawnAtRot = base.transform.rotation;
		this.currentState = TransferrableObject.PositionState.Dropped;
	}

	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		base.OnGrab(pointGrabbed, grabbingHand);
		this.inInitialPose = false;
		this.StopRespawnTimer();
	}

	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		this.StartRespawnTimer(-1f);
		return true;
	}

	public override void DropItem()
	{
		base.DropItem();
		this.StartRespawnTimer(-1f);
	}

	private void StopRespawnTimer()
	{
		if (this.respawnTimer != null)
		{
			base.StopCoroutine(this.respawnTimer);
			this.respawnTimer = null;
		}
	}

	public void SetWillTeleport()
	{
		this.worldShareableInstance.SetWillTeleport();
	}

	public void StartRespawnTimer(float overrideTimer = -1f)
	{
		float timerDuration = (overrideTimer != -1f) ? overrideTimer : this.respawnTimerDuration;
		this.StopRespawnTimer();
		if (this.respawnTimerDuration != 0f && (base.photonView == null || base.photonView.IsMine))
		{
			this.respawnTimer = base.StartCoroutine(this.RespawnTimerCoroutine(timerDuration));
		}
	}

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

	public ThrowableSetDressing()
	{
	}

	public float respawnTimerDuration;

	[CompilerGenerated]
	private bool <inInitialPose>k__BackingField;

	[Tooltip("set this only if this set dressing is using as an ingredient for the magic cauldron - Halloween")]
	public MagicIngredientType IngredientTypeSO;

	private float _respawnTimestamp;

	[SerializeField]
	private CapsuleCollider capsuleCollider;

	private Vector3 respawnAtPos;

	private Quaternion respawnAtRot;

	private Coroutine respawnTimer;

	[CompilerGenerated]
	private sealed class <RespawnTimerCoroutine>d__19 : IEnumerator<object>, IEnumerator, IDisposable
	{
		[DebuggerHidden]
		public <RespawnTimerCoroutine>d__19(int <>1__state)
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
			ThrowableSetDressing throwableSetDressing = this;
			if (num == 0)
			{
				this.<>1__state = -1;
				this.<>2__current = new WaitForSeconds(timerDuration);
				this.<>1__state = 1;
				return true;
			}
			if (num != 1)
			{
				return false;
			}
			this.<>1__state = -1;
			if (throwableSetDressing.InHand())
			{
				return false;
			}
			throwableSetDressing.SetWillTeleport();
			throwableSetDressing.transform.position = throwableSetDressing.respawnAtPos;
			throwableSetDressing.transform.rotation = throwableSetDressing.respawnAtRot;
			throwableSetDressing.inInitialPose = true;
			throwableSetDressing.rigidbodyInstance.isKinematic = true;
			return false;
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

		public float timerDuration;

		public ThrowableSetDressing <>4__this;
	}
}
