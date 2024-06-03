﻿using System;
using GorillaExtensions;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;

namespace GorillaTagScripts
{
	public class DecorativeItemReliableState : MonoBehaviour, IPunObservable
	{
		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			if (stream.IsWriting)
			{
				stream.SendNext(this.isSnapped);
				stream.SendNext(this.snapPosition);
				stream.SendNext(this.respawnPosition);
				stream.SendNext(this.respawnRotation);
				return;
			}
			this.isSnapped = (bool)stream.ReceiveNext();
			this.snapPosition = (Vector3)stream.ReceiveNext();
			this.respawnPosition = (Vector3)stream.ReceiveNext();
			this.respawnRotation = (Quaternion)stream.ReceiveNext();
			if (!this.snapPosition.IsValid())
			{
				this.snapPosition = Vector3.zero;
			}
			if (!this.respawnPosition.IsValid())
			{
				this.respawnPosition = Vector3.zero;
			}
			if (!this.respawnRotation.IsValid())
			{
				this.respawnRotation = quaternion.identity;
			}
		}

		public DecorativeItemReliableState()
		{
		}

		public bool isSnapped;

		public Vector3 snapPosition = Vector3.zero;

		public Vector3 respawnPosition = Vector3.zero;

		public Quaternion respawnRotation = Quaternion.identity;
	}
}
