using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Photon.Pun;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	public class RopeSwingManager : MonoBehaviourPun
	{
		public static RopeSwingManager instance
		{
			[CompilerGenerated]
			get
			{
				return RopeSwingManager.<instance>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				RopeSwingManager.<instance>k__BackingField = value;
			}
		}

		private void Awake()
		{
			if (RopeSwingManager.instance != null && RopeSwingManager.instance != this)
			{
				GTDev.LogWarning("Instance of RopeSwingManager already exists. Destroying.", null);
				Object.Destroy(this);
				return;
			}
			if (RopeSwingManager.instance == null)
			{
				RopeSwingManager.instance = this;
			}
		}

		private void RegisterInstance(GorillaRopeSwing t)
		{
			this.ropes.Add(t.ropeId, t);
		}

		private void UnregisterInstance(GorillaRopeSwing t)
		{
			this.ropes.Remove(t.ropeId);
		}

		public static void Register(GorillaRopeSwing t)
		{
			RopeSwingManager.instance.RegisterInstance(t);
		}

		public static void Unregister(GorillaRopeSwing t)
		{
			RopeSwingManager.instance.UnregisterInstance(t);
		}

		public void SendSetVelocity_RPC(int ropeId, int boneIndex, Vector3 velocity, bool wholeRope)
		{
			if (PhotonNetwork.InRoom)
			{
				base.photonView.RPC("SetVelocity", RpcTarget.All, new object[]
				{
					ropeId,
					boneIndex,
					velocity,
					wholeRope
				});
				return;
			}
			this.SetVelocity(ropeId, boneIndex, velocity, wholeRope, default(PhotonMessageInfo));
		}

		public GorillaRopeSwing GetRope(int ropeId)
		{
			GorillaRopeSwing result;
			this.ropes.TryGetValue(ropeId, out result);
			return result;
		}

		[PunRPC]
		public void SetVelocity(int ropeId, int boneIndex, Vector3 velocity, bool wholeRope, PhotonMessageInfo info)
		{
			if (info.Sender != null)
			{
				GorillaNot.IncrementRPCCall(info, "SetVelocity");
			}
			GorillaRopeSwing rope = this.GetRope(ropeId);
			if (rope != null)
			{
				rope.SetVelocity(boneIndex, velocity, wholeRope, info);
			}
		}

		public RopeSwingManager()
		{
		}

		[CompilerGenerated]
		private static RopeSwingManager <instance>k__BackingField;

		private Dictionary<int, GorillaRopeSwing> ropes = new Dictionary<int, GorillaRopeSwing>();
	}
}
