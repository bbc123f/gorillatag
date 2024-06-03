using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Photon.Pun;
using UnityEngine;

public class ChestHeartbeat : MonoBehaviour
{
	public void Update()
	{
		if (PhotonNetwork.InRoom)
		{
			if ((PhotonNetwork.ServerTimestamp > this.lastShot + this.millisMin || Mathf.Abs(PhotonNetwork.ServerTimestamp - this.lastShot) > 10000) && PhotonNetwork.ServerTimestamp % 1500 <= 10)
			{
				this.lastShot = PhotonNetwork.ServerTimestamp;
				this.audioSource.PlayOneShot(this.audioSource.clip);
				base.StartCoroutine(this.HeartBeat());
				return;
			}
		}
		else if ((Time.time * 1000f > (float)(this.lastShot + this.millisMin) || Mathf.Abs(Time.time * 1000f - (float)this.lastShot) > 10000f) && Time.time * 1000f % 1500f <= 10f)
		{
			this.lastShot = PhotonNetwork.ServerTimestamp;
			this.audioSource.PlayOneShot(this.audioSource.clip);
			base.StartCoroutine(this.HeartBeat());
		}
	}

	private IEnumerator HeartBeat()
	{
		float startTime = Time.time;
		while (Time.time < startTime + this.endtime)
		{
			if (Time.time < startTime + this.minTime)
			{
				this.deltaTime = Time.time - startTime;
				this.scaleTransform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * this.heartMinSize, this.deltaTime / this.minTime);
			}
			else if (Time.time < startTime + this.maxTime)
			{
				this.deltaTime = Time.time - startTime - this.minTime;
				this.scaleTransform.localScale = Vector3.Lerp(Vector3.one * this.heartMinSize, Vector3.one * this.heartMaxSize, this.deltaTime / (this.maxTime - this.minTime));
			}
			else if (Time.time < startTime + this.endtime)
			{
				this.deltaTime = Time.time - startTime - this.maxTime;
				this.scaleTransform.localScale = Vector3.Lerp(Vector3.one * this.heartMaxSize, Vector3.one, this.deltaTime / (this.endtime - this.maxTime));
			}
			yield return new WaitForFixedUpdate();
		}
		yield break;
	}

	public ChestHeartbeat()
	{
	}

	public int millisToWait;

	public int millisMin = 300;

	public int lastShot;

	public AudioSource audioSource;

	public Transform scaleTransform;

	private float deltaTime;

	private float heartMinSize = 0.9f;

	private float heartMaxSize = 1.2f;

	private float minTime = 0.05f;

	private float maxTime = 0.1f;

	private float endtime = 0.25f;

	private float currentTime;

	[CompilerGenerated]
	private sealed class <HeartBeat>d__13 : IEnumerator<object>, IEnumerator, IDisposable
	{
		[DebuggerHidden]
		public <HeartBeat>d__13(int <>1__state)
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
			ChestHeartbeat chestHeartbeat = this;
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
				startTime = Time.time;
			}
			if (Time.time >= startTime + chestHeartbeat.endtime)
			{
				return false;
			}
			if (Time.time < startTime + chestHeartbeat.minTime)
			{
				chestHeartbeat.deltaTime = Time.time - startTime;
				chestHeartbeat.scaleTransform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * chestHeartbeat.heartMinSize, chestHeartbeat.deltaTime / chestHeartbeat.minTime);
			}
			else if (Time.time < startTime + chestHeartbeat.maxTime)
			{
				chestHeartbeat.deltaTime = Time.time - startTime - chestHeartbeat.minTime;
				chestHeartbeat.scaleTransform.localScale = Vector3.Lerp(Vector3.one * chestHeartbeat.heartMinSize, Vector3.one * chestHeartbeat.heartMaxSize, chestHeartbeat.deltaTime / (chestHeartbeat.maxTime - chestHeartbeat.minTime));
			}
			else if (Time.time < startTime + chestHeartbeat.endtime)
			{
				chestHeartbeat.deltaTime = Time.time - startTime - chestHeartbeat.maxTime;
				chestHeartbeat.scaleTransform.localScale = Vector3.Lerp(Vector3.one * chestHeartbeat.heartMaxSize, Vector3.one, chestHeartbeat.deltaTime / (chestHeartbeat.endtime - chestHeartbeat.maxTime));
			}
			this.<>2__current = new WaitForFixedUpdate();
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

		public ChestHeartbeat <>4__this;

		private float <startTime>5__2;
	}
}
