﻿using System;
using System.Collections.Generic;
using System.Linq;
using GorillaExtensions;
using GorillaLocomotion.Climbing;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x02000298 RID: 664
	public class GorillaRopeSwing : MonoBehaviourPun
	{
		// Token: 0x17000102 RID: 258
		// (get) Token: 0x06001148 RID: 4424 RVA: 0x0006131C File Offset: 0x0005F51C
		// (set) Token: 0x06001149 RID: 4425 RVA: 0x00061324 File Offset: 0x0005F524
		public bool isIdle { get; private set; }

		// Token: 0x17000103 RID: 259
		// (get) Token: 0x0600114A RID: 4426 RVA: 0x0006132D File Offset: 0x0005F52D
		// (set) Token: 0x0600114B RID: 4427 RVA: 0x00061335 File Offset: 0x0005F535
		public bool isFullyIdle { get; private set; }

		// Token: 0x17000104 RID: 260
		// (get) Token: 0x0600114C RID: 4428 RVA: 0x0006133E File Offset: 0x0005F53E
		public bool hasPlayers
		{
			get
			{
				return this.localPlayerOn || this.remotePlayers.Count > 0;
			}
		}

		// Token: 0x0600114D RID: 4429 RVA: 0x00061358 File Offset: 0x0005F558
		private void Awake()
		{
			base.transform.rotation = Quaternion.identity;
			this.scaleFactor = (base.transform.localScale.x + base.transform.localScale.y + base.transform.localScale.z) / 3f;
			this.SetIsIdle(true, false);
		}

		// Token: 0x0600114E RID: 4430 RVA: 0x000613BB File Offset: 0x0005F5BB
		private void OnEnable()
		{
			VectorizedCustomRopeSimulation.Register(this);
		}

		// Token: 0x0600114F RID: 4431 RVA: 0x000613C3 File Offset: 0x0005F5C3
		private void OnDisable()
		{
			if (!this.isIdle)
			{
				this.SetIsIdle(true, true);
			}
			VectorizedCustomRopeSimulation.Unregister(this);
		}

		// Token: 0x06001150 RID: 4432 RVA: 0x000613DC File Offset: 0x0005F5DC
		private void Update()
		{
			if (this.isIdle)
			{
				this.isFullyIdle = true;
			}
			if (!this.isIdle)
			{
				int num = -1;
				if (this.localPlayerOn)
				{
					num = this.localPlayerBoneIndex;
				}
				else if (this.remotePlayers.Count > 0)
				{
					num = this.remotePlayers.First<KeyValuePair<int, int>>().Value;
				}
				if (num >= 0 && VectorizedCustomRopeSimulation.instance.GetNodeVelocity(this, num).magnitude > 2f && !this.ropeCreakSFX.isPlaying && Mathf.RoundToInt(Time.time) % 5 == 0)
				{
					this.ropeCreakSFX.Play();
				}
				if (this.localPlayerOn)
				{
					float num2 = MathUtils.Linear(this.velocityTracker.GetLatestVelocity(true).magnitude / this.scaleFactor, 0f, 10f, -0.07f, 0.5f);
					if (num2 > 0f)
					{
						GorillaTagger.Instance.DoVibration(this.localPlayerXRNode, num2, Time.deltaTime);
					}
				}
				Transform bone = this.GetBone(this.lastNodeCheckIndex);
				Vector3 nodeVelocity = VectorizedCustomRopeSimulation.instance.GetNodeVelocity(this, this.lastNodeCheckIndex);
				if (Physics.SphereCastNonAlloc(bone.position, 0.2f * this.scaleFactor, nodeVelocity.normalized, this.nodeHits, 0.4f * this.scaleFactor, this.wallLayerMask, QueryTriggerInteraction.Ignore) > 0)
				{
					this.SetVelocity(this.lastNodeCheckIndex, Vector3.zero, false, default(PhotonMessageInfo));
				}
				if (nodeVelocity.magnitude <= 0.35f)
				{
					this.potentialIdleTimer += Time.deltaTime;
				}
				else
				{
					this.potentialIdleTimer = 0f;
				}
				if (this.potentialIdleTimer >= 2f)
				{
					this.SetIsIdle(true, false);
					this.potentialIdleTimer = 0f;
				}
				this.lastNodeCheckIndex++;
				if (this.lastNodeCheckIndex > this.nodes.Length)
				{
					this.lastNodeCheckIndex = 2;
				}
			}
		}

		// Token: 0x06001151 RID: 4433 RVA: 0x000615C8 File Offset: 0x0005F7C8
		private void SetIsIdle(bool idle, bool resetPos = false)
		{
			this.isIdle = idle;
			this.ropeCreakSFX.gameObject.SetActive(!idle);
			if (idle)
			{
				this.ToggleVelocityTracker(false, 0, default(Vector3));
				if (resetPos)
				{
					Vector3 vector = Vector3.zero;
					for (int i = 0; i < this.nodes.Length; i++)
					{
						this.nodes[i].transform.localRotation = Quaternion.identity;
						this.nodes[i].transform.localPosition = vector;
						vector += new Vector3(0f, -1f, 0f);
					}
					return;
				}
			}
			else
			{
				this.isFullyIdle = false;
			}
		}

		// Token: 0x06001152 RID: 4434 RVA: 0x0006166D File Offset: 0x0005F86D
		public Transform GetBone(int index)
		{
			if (index >= this.nodes.Length)
			{
				return this.nodes.Last<Transform>();
			}
			return this.nodes[index];
		}

		// Token: 0x06001153 RID: 4435 RVA: 0x00061690 File Offset: 0x0005F890
		public int GetBoneIndex(Transform r)
		{
			for (int i = 0; i < this.nodes.Length; i++)
			{
				if (this.nodes[i] == r)
				{
					return i;
				}
			}
			return this.nodes.Length - 1;
		}

		// Token: 0x06001154 RID: 4436 RVA: 0x000616CC File Offset: 0x0005F8CC
		public void AttachLocalPlayer(XRNode xrNode, Transform grabbedBone, Vector3 offset, Vector3 velocity)
		{
			int boneIndex = this.GetBoneIndex(grabbedBone);
			this.localPlayerBoneIndex = boneIndex;
			velocity /= this.scaleFactor;
			velocity *= this.settings.inheritVelocityMultiplier;
			if (GorillaTagger.hasInstance && GorillaTagger.Instance.offlineVRRig)
			{
				GorillaTagger.Instance.offlineVRRig.grabbedRopeIndex = base.photonView.ViewID;
				GorillaTagger.Instance.offlineVRRig.grabbedRopeBoneIndex = boneIndex;
				GorillaTagger.Instance.offlineVRRig.grabbedRopeIsLeft = (xrNode == XRNode.LeftHand);
				GorillaTagger.Instance.offlineVRRig.grabbedRopeOffset = offset;
			}
			this.RefreshAllBonesMass();
			List<Vector3> list = new List<Vector3>();
			if (this.remotePlayers.Count <= 0)
			{
				foreach (Transform transform in this.nodes)
				{
					list.Add(transform.position);
				}
			}
			velocity.y = 0f;
			if (Time.time - this.lastGrabTime > 1f && (this.remotePlayers.Count == 0 || velocity.magnitude > 2.5f))
			{
				this.SetVelocity_RPC(boneIndex, velocity, true);
			}
			this.lastGrabTime = Time.time;
			this.ropeCreakSFX.transform.parent = this.GetBone(Math.Max(0, boneIndex - 3)).transform;
			this.ropeCreakSFX.transform.localPosition = Vector3.zero;
			this.localPlayerOn = true;
			this.localPlayerXRNode = xrNode;
			this.ToggleVelocityTracker(true, boneIndex, offset);
		}

		// Token: 0x06001155 RID: 4437 RVA: 0x00061850 File Offset: 0x0005FA50
		public void DetachLocalPlayer()
		{
			if (GorillaTagger.hasInstance && GorillaTagger.Instance.offlineVRRig)
			{
				GorillaTagger.Instance.offlineVRRig.grabbedRopeIndex = -1;
			}
			this.localPlayerOn = false;
			this.localPlayerBoneIndex = 0;
			this.RefreshAllBonesMass();
		}

		// Token: 0x06001156 RID: 4438 RVA: 0x00061890 File Offset: 0x0005FA90
		private void ToggleVelocityTracker(bool enable, int boneIndex = 0, Vector3 offset = default(Vector3))
		{
			if (enable)
			{
				this.velocityTracker.transform.SetParent(this.GetBone(boneIndex));
				this.velocityTracker.transform.localPosition = offset;
				this.velocityTracker.ResetState();
			}
			this.velocityTracker.gameObject.SetActive(enable);
			if (enable)
			{
				this.velocityTracker.Tick();
			}
		}

		// Token: 0x06001157 RID: 4439 RVA: 0x000618F4 File Offset: 0x0005FAF4
		private void RefreshAllBonesMass()
		{
			int num = 0;
			foreach (KeyValuePair<int, int> keyValuePair in this.remotePlayers)
			{
				if (keyValuePair.Value > num)
				{
					num = keyValuePair.Value;
				}
			}
			if (this.localPlayerBoneIndex > num)
			{
				num = this.localPlayerBoneIndex;
			}
			VectorizedCustomRopeSimulation.instance.SetMassForPlayers(this, this.hasPlayers, num);
		}

		// Token: 0x06001158 RID: 4440 RVA: 0x00061978 File Offset: 0x0005FB78
		public bool AttachRemotePlayer(int playerId, int boneIndex, Transform offsetTransform, Vector3 offset)
		{
			Transform bone = this.GetBone(boneIndex);
			if (bone == null)
			{
				return false;
			}
			offsetTransform.SetParent(bone.transform);
			offsetTransform.localPosition = offset;
			offsetTransform.localRotation = Quaternion.identity;
			if (this.remotePlayers.ContainsKey(playerId))
			{
				Debug.LogError("already on the list!");
				return false;
			}
			this.remotePlayers.Add(playerId, boneIndex);
			this.RefreshAllBonesMass();
			return true;
		}

		// Token: 0x06001159 RID: 4441 RVA: 0x000619E5 File Offset: 0x0005FBE5
		public void DetachRemotePlayer(int playerId)
		{
			this.remotePlayers.Remove(playerId);
			this.RefreshAllBonesMass();
		}

		// Token: 0x0600115A RID: 4442 RVA: 0x000619FC File Offset: 0x0005FBFC
		public void SetVelocity_RPC(int boneIndex, Vector3 velocity, bool wholeRope)
		{
			if (PhotonNetwork.InRoom)
			{
				base.photonView.RPC("SetVelocity", RpcTarget.All, new object[]
				{
					boneIndex,
					velocity,
					wholeRope
				});
				return;
			}
			this.SetVelocity(boneIndex, velocity, wholeRope, default(PhotonMessageInfo));
		}

		// Token: 0x0600115B RID: 4443 RVA: 0x00061A58 File Offset: 0x0005FC58
		[PunRPC]
		public void SetVelocity(int boneIndex, Vector3 velocity, bool wholeRope, PhotonMessageInfo info)
		{
			if (info.Sender != null)
			{
				GorillaNot.IncrementRPCCall(info, "SetVelocity");
			}
			if (!base.isActiveAndEnabled)
			{
				return;
			}
			if (!velocity.IsValid())
			{
				return;
			}
			boneIndex = Mathf.Clamp(boneIndex, 0, this.nodes.Length);
			Transform bone = this.GetBone(boneIndex);
			if (!bone)
			{
				return;
			}
			if (info.Sender != null && !info.Sender.IsLocal)
			{
				VRRig vrrig = GorillaGameManager.StaticFindRigForPlayer(info.Sender);
				if (!vrrig || Vector3.Distance(bone.position, vrrig.transform.position) > 5f)
				{
					return;
				}
			}
			this.SetIsIdle(false, false);
			if (bone)
			{
				VectorizedCustomRopeSimulation.instance.SetVelocity(this, velocity, wholeRope, boneIndex);
			}
		}

		// Token: 0x040013CF RID: 5071
		public const float ropeBitGenOffset = 1f;

		// Token: 0x040013D0 RID: 5072
		[SerializeField]
		private GameObject prefabRopeBit;

		// Token: 0x040013D1 RID: 5073
		public Transform[] nodes = Array.Empty<Transform>();

		// Token: 0x040013D2 RID: 5074
		private Dictionary<int, int> remotePlayers = new Dictionary<int, int>();

		// Token: 0x040013D3 RID: 5075
		[NonSerialized]
		public float lastGrabTime;

		// Token: 0x040013D4 RID: 5076
		[SerializeField]
		private AudioSource ropeCreakSFX;

		// Token: 0x040013D5 RID: 5077
		public GorillaVelocityTracker velocityTracker;

		// Token: 0x040013D6 RID: 5078
		private bool localPlayerOn;

		// Token: 0x040013D7 RID: 5079
		private int localPlayerBoneIndex;

		// Token: 0x040013D8 RID: 5080
		private XRNode localPlayerXRNode;

		// Token: 0x040013D9 RID: 5081
		private const float MAX_VELOCITY_FOR_IDLE = 0.5f;

		// Token: 0x040013DA RID: 5082
		private const float TIME_FOR_IDLE = 2f;

		// Token: 0x040013DD RID: 5085
		private float potentialIdleTimer;

		// Token: 0x040013DE RID: 5086
		[SerializeField]
		private int ropeLength = 8;

		// Token: 0x040013DF RID: 5087
		[SerializeField]
		private GorillaRopeSwingSettings settings;

		// Token: 0x040013E0 RID: 5088
		[NonSerialized]
		public int ropeDataStartIndex;

		// Token: 0x040013E1 RID: 5089
		[NonSerialized]
		public int ropeDataIndexOffset;

		// Token: 0x040013E2 RID: 5090
		[SerializeField]
		private LayerMask wallLayerMask;

		// Token: 0x040013E3 RID: 5091
		private RaycastHit[] nodeHits = new RaycastHit[1];

		// Token: 0x040013E4 RID: 5092
		private float scaleFactor = 1f;

		// Token: 0x040013E5 RID: 5093
		private int lastNodeCheckIndex = 2;
	}
}
