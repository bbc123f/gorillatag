using System;
using Photon.Pun;
using UnityEngine;

public class SecondLookSkeletonEnabler : Tappable
{
	private void Awake()
	{
		this.isTapped = false;
		this.skele = Object.FindFirstObjectByType<SecondLookSkeleton>();
		this.skele.spookyText = this.spookyText;
	}

	public override void OnTapLocal(float tapStrength, float tapTime, PhotonMessageInfo sender)
	{
		if (!this.isTapped)
		{
			base.OnTapLocal(tapStrength, tapTime, sender);
			if (this.skele != null)
			{
				this.skele.tapped = true;
			}
			base.gameObject.SetActive(false);
			this.isTapped = true;
			this.playOnDisappear.Play();
			this.particles.Play();
		}
	}

	public SecondLookSkeletonEnabler()
	{
	}

	public bool isTapped;

	public AudioSource playOnDisappear;

	public ParticleSystem particles;

	public GameObject spookyText;

	private SecondLookSkeleton skele;
}
