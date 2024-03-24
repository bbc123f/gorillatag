using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using GorillaTag;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class HitTargetNetworkState : MonoBehaviourPunCallbacks, IPunObservable
{
	protected void Awake()
	{
		this.audioPlayer = base.GetComponent<AudioSource>();
		SlingshotProjectileHitNotifier component = base.GetComponent<SlingshotProjectileHitNotifier>();
		if (component != null)
		{
			component.OnProjectileHit += this.ProjectileHitReciever;
			component.OnProjectileCollisionStay += this.ProjectileHitReciever;
			return;
		}
		Debug.LogError("Needs SlingshotProjectileHitNotifier added to this GameObject to increment score");
	}

	private void SetInitialState()
	{
		this.networkedScore.Value = 0;
		this.nextHittableTimestamp = 0f;
		this.audioPlayer.Stop();
	}

	public override void OnDisconnected(DisconnectCause cause)
	{
		this.OnLeftRoom();
	}

	public override void OnLeftRoom()
	{
		base.OnLeftRoom();
		this.SetInitialState();
	}

	public override void OnEnable()
	{
		base.OnEnable();
		if (Application.isEditor)
		{
			base.StartCoroutine(this.TestPressCheck());
		}
		this.SetInitialState();
	}

	private IEnumerator TestPressCheck()
	{
		for (;;)
		{
			if (this.testPress)
			{
				this.testPress = false;
				this.TargetHit();
			}
			yield return new WaitForSeconds(1f);
		}
		yield break;
	}

	private void ProjectileHitReciever(SlingshotProjectile projectile, Collision collision)
	{
		this.TargetHit();
	}

	public void TargetHit()
	{
		if (PhotonNetwork.IsMasterClient && Time.time >= this.nextHittableTimestamp)
		{
			int num = this.networkedScore.Value;
			num++;
			if (num >= 1000)
			{
				num = 0;
			}
			this.PlayAudio(this.networkedScore.Value, num);
			this.networkedScore.Value = num;
			this.nextHittableTimestamp = Time.time + (float)this.hitCooldownTime;
		}
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		if (stream.IsWriting)
		{
			stream.SendNext(this.networkedScore.Value);
			return;
		}
		int num = (int)stream.ReceiveNext();
		if (num != this.networkedScore.Value)
		{
			this.PlayAudio(this.networkedScore.Value, num);
		}
		this.networkedScore.Value = num;
	}

	public void PlayAudio(int oldScore, int newScore)
	{
		if (oldScore > newScore)
		{
			this.audioPlayer.PlayOneShot(this.audioClips[1]);
			return;
		}
		this.audioPlayer.PlayOneShot(this.audioClips[0]);
	}

	public HitTargetNetworkState()
	{
	}

	[SerializeField]
	private WatchableIntSO networkedScore;

	[SerializeField]
	private int hitCooldownTime = 1;

	[SerializeField]
	private bool testPress;

	[SerializeField]
	private AudioClip[] audioClips;

	private AudioSource audioPlayer;

	private float nextHittableTimestamp;

	[CompilerGenerated]
	private sealed class <TestPressCheck>d__11 : IEnumerator<object>, IEnumerator, IDisposable
	{
		[DebuggerHidden]
		public <TestPressCheck>d__11(int <>1__state)
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
			HitTargetNetworkState hitTargetNetworkState = this;
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
			if (hitTargetNetworkState.testPress)
			{
				hitTargetNetworkState.testPress = false;
				hitTargetNetworkState.TargetHit();
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

		public HitTargetNetworkState <>4__this;
	}
}
