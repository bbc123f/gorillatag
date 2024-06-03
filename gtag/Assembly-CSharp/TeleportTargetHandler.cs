using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class TeleportTargetHandler : TeleportSupport
{
	protected TeleportTargetHandler()
	{
		this._startAimAction = delegate()
		{
			base.StartCoroutine(this.TargetAimCoroutine());
		};
	}

	protected override void AddEventHandlers()
	{
		base.AddEventHandlers();
		base.LocomotionTeleport.EnterStateAim += this._startAimAction;
	}

	protected override void RemoveEventHandlers()
	{
		base.RemoveEventHandlers();
		base.LocomotionTeleport.EnterStateAim -= this._startAimAction;
	}

	private IEnumerator TargetAimCoroutine()
	{
		while (base.LocomotionTeleport.CurrentState == LocomotionTeleport.States.Aim)
		{
			this.ResetAimData();
			Vector3 start = base.LocomotionTeleport.transform.position;
			this._aimPoints.Clear();
			base.LocomotionTeleport.AimHandler.GetPoints(this._aimPoints);
			for (int i = 0; i < this._aimPoints.Count; i++)
			{
				Vector3 vector = this._aimPoints[i];
				this.AimData.TargetValid = this.ConsiderTeleport(start, ref vector);
				this.AimData.Points.Add(vector);
				if (this.AimData.TargetValid)
				{
					this.AimData.Destination = this.ConsiderDestination(vector);
					this.AimData.TargetValid = (this.AimData.Destination != null);
					break;
				}
				start = this._aimPoints[i];
			}
			base.LocomotionTeleport.OnUpdateAimData(this.AimData);
			yield return null;
		}
		yield break;
	}

	protected virtual void ResetAimData()
	{
		this.AimData.Reset();
	}

	protected abstract bool ConsiderTeleport(Vector3 start, ref Vector3 end);

	public virtual Vector3? ConsiderDestination(Vector3 location)
	{
		CapsuleCollider characterController = base.LocomotionTeleport.LocomotionController.CharacterController;
		float num = characterController.radius - 0.1f;
		Vector3 vector = location;
		vector.y += num + 0.1f;
		Vector3 end = vector;
		end.y += characterController.height - 0.1f;
		if (Physics.CheckCapsule(vector, end, num, this.AimCollisionLayerMask, QueryTriggerInteraction.Ignore))
		{
			return null;
		}
		return new Vector3?(location);
	}

	[CompilerGenerated]
	private void <.ctor>b__3_0()
	{
		base.StartCoroutine(this.TargetAimCoroutine());
	}

	[Tooltip("This bitmask controls which game object layers will be included in the targeting collision tests.")]
	public LayerMask AimCollisionLayerMask;

	protected readonly LocomotionTeleport.AimData AimData = new LocomotionTeleport.AimData();

	private readonly Action _startAimAction;

	private readonly List<Vector3> _aimPoints = new List<Vector3>();

	private const float ERROR_MARGIN = 0.1f;

	[CompilerGenerated]
	private sealed class <TargetAimCoroutine>d__7 : IEnumerator<object>, IEnumerator, IDisposable
	{
		[DebuggerHidden]
		public <TargetAimCoroutine>d__7(int <>1__state)
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
			TeleportTargetHandler teleportTargetHandler = this;
			if (num != 0)
			{
				if (num != 1)
				{
					return false;
				}
				this.<>1__state = -1;
			}
			else
			{
				this.<>1__state = -1;
			}
			if (teleportTargetHandler.LocomotionTeleport.CurrentState != LocomotionTeleport.States.Aim)
			{
				return false;
			}
			teleportTargetHandler.ResetAimData();
			Vector3 start = teleportTargetHandler.LocomotionTeleport.transform.position;
			teleportTargetHandler._aimPoints.Clear();
			teleportTargetHandler.LocomotionTeleport.AimHandler.GetPoints(teleportTargetHandler._aimPoints);
			for (int i = 0; i < teleportTargetHandler._aimPoints.Count; i++)
			{
				Vector3 vector = teleportTargetHandler._aimPoints[i];
				teleportTargetHandler.AimData.TargetValid = teleportTargetHandler.ConsiderTeleport(start, ref vector);
				teleportTargetHandler.AimData.Points.Add(vector);
				if (teleportTargetHandler.AimData.TargetValid)
				{
					teleportTargetHandler.AimData.Destination = teleportTargetHandler.ConsiderDestination(vector);
					teleportTargetHandler.AimData.TargetValid = (teleportTargetHandler.AimData.Destination != null);
					break;
				}
				start = teleportTargetHandler._aimPoints[i];
			}
			teleportTargetHandler.LocomotionTeleport.OnUpdateAimData(teleportTargetHandler.AimData);
			this.<>2__current = null;
			this.<>1__state = 1;
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

		public TeleportTargetHandler <>4__this;
	}
}
