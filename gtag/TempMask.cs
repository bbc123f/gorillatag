using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using GorillaNetworking;
using UnityEngine;

public class TempMask : MonoBehaviour
{
	private void Awake()
	{
		this.dayOn = new DateTime(this.year, this.month, this.day);
		this.myRig = base.GetComponentInParent<VRRig>();
		if (this.myRig != null && this.myRig.photonView.IsMine && !this.myRig.isOfflineVRRig)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void OnEnable()
	{
		base.StartCoroutine(this.MaskOnDuringDate());
	}

	private void OnDisable()
	{
		base.StopAllCoroutines();
	}

	private IEnumerator MaskOnDuringDate()
	{
		for (;;)
		{
			if (GorillaComputer.instance != null && GorillaComputer.instance.startupMillis != 0L)
			{
				this.myDate = new DateTime(GorillaComputer.instance.startupMillis * 10000L + (long)(Time.realtimeSinceStartup * 1000f * 10000f)).Subtract(TimeSpan.FromHours(7.0));
				if (this.myDate.DayOfYear == this.dayOn.DayOfYear)
				{
					if (!this.myRenderer.enabled)
					{
						this.myRenderer.enabled = true;
					}
				}
				else if (this.myRenderer.enabled)
				{
					this.myRenderer.enabled = false;
				}
			}
			yield return new WaitForSeconds(1f);
		}
		yield break;
	}

	public TempMask()
	{
	}

	public int year;

	public int month;

	public int day;

	public DateTime dayOn;

	public MeshRenderer myRenderer;

	private DateTime myDate;

	private VRRig myRig;

	[CompilerGenerated]
	private sealed class <MaskOnDuringDate>d__10 : IEnumerator<object>, IEnumerator, IDisposable
	{
		[DebuggerHidden]
		public <MaskOnDuringDate>d__10(int <>1__state)
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
			TempMask tempMask = this;
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
			if (GorillaComputer.instance != null && GorillaComputer.instance.startupMillis != 0L)
			{
				tempMask.myDate = new DateTime(GorillaComputer.instance.startupMillis * 10000L + (long)(Time.realtimeSinceStartup * 1000f * 10000f)).Subtract(TimeSpan.FromHours(7.0));
				if (tempMask.myDate.DayOfYear == tempMask.dayOn.DayOfYear)
				{
					if (!tempMask.myRenderer.enabled)
					{
						tempMask.myRenderer.enabled = true;
					}
				}
				else if (tempMask.myRenderer.enabled)
				{
					tempMask.myRenderer.enabled = false;
				}
			}
			this.<>2__current = new WaitForSeconds(1f);
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

		public TempMask <>4__this;
	}
}
