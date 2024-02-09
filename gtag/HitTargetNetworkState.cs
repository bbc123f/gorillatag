using System;
using System.Collections;
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
		if (stream.IsWriting)
		{
			stream.SendNext(this.networkedScore.Value);
			return;
		}
		int num = (int)stream.ReceiveNext();
		this.PlayAudio(this.networkedScore.Value, num);
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
}
