using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ThrowableBugBeaconActivation : MonoBehaviour
{
	private void Awake()
	{
		this.tbb = base.GetComponent<ThrowableBugBeacon>();
	}

	private void OnEnable()
	{
		base.StartCoroutine(this.SendSignals());
	}

	private void OnDisable()
	{
		base.StopAllCoroutines();
	}

	private IEnumerator SendSignals()
	{
		uint count = 0U;
		while (this.signalCount == 0U || count < this.signalCount)
		{
			yield return new WaitForSeconds(Random.Range(this.minCallTime, this.maxCallTime));
			switch (this.mode)
			{
			case ThrowableBugBeaconActivation.ActivationMode.CALL:
				this.tbb.Call();
				break;
			case ThrowableBugBeaconActivation.ActivationMode.DISMISS:
				this.tbb.Dismiss();
				break;
			case ThrowableBugBeaconActivation.ActivationMode.LOCK:
				this.tbb.Lock();
				break;
			}
			uint num = count;
			count = num + 1U;
		}
		yield break;
	}

	public ThrowableBugBeaconActivation()
	{
	}

	[SerializeField]
	private float minCallTime = 1f;

	[SerializeField]
	private float maxCallTime = 5f;

	[SerializeField]
	private uint signalCount;

	[SerializeField]
	private ThrowableBugBeaconActivation.ActivationMode mode;

	private ThrowableBugBeacon tbb;

	private enum ActivationMode
	{
		CALL,
		DISMISS,
		LOCK
	}

	[CompilerGenerated]
	private sealed class <SendSignals>d__9 : IEnumerator<object>, IEnumerator, IDisposable
	{
		[DebuggerHidden]
		public <SendSignals>d__9(int <>1__state)
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
			ThrowableBugBeaconActivation throwableBugBeaconActivation = this;
			if (num != 0)
			{
				if (num != 1)
				{
					return false;
				}
				this.<>1__state = -1;
				switch (throwableBugBeaconActivation.mode)
				{
				case ThrowableBugBeaconActivation.ActivationMode.CALL:
					throwableBugBeaconActivation.tbb.Call();
					break;
				case ThrowableBugBeaconActivation.ActivationMode.DISMISS:
					throwableBugBeaconActivation.tbb.Dismiss();
					break;
				case ThrowableBugBeaconActivation.ActivationMode.LOCK:
					throwableBugBeaconActivation.tbb.Lock();
					break;
				}
				uint num2 = count;
				count = num2 + 1U;
			}
			else
			{
				this.<>1__state = -1;
				count = 0U;
			}
			if (throwableBugBeaconActivation.signalCount != 0U && count >= throwableBugBeaconActivation.signalCount)
			{
				return false;
			}
			this.<>2__current = new WaitForSeconds(Random.Range(throwableBugBeaconActivation.minCallTime, throwableBugBeaconActivation.maxCallTime));
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

		public ThrowableBugBeaconActivation <>4__this;

		private uint <count>5__2;
	}
}
