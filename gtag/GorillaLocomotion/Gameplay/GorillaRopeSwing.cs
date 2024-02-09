using System;
using System.Collections.Generic;
using System.Linq;
using GorillaExtensions;
using GorillaLocomotion.Climbing;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

namespace GorillaLocomotion.Gameplay
{
	public class GorillaRopeSwing : MonoBehaviourPun
	{
		public bool isIdle { get; private set; }

		public bool isFullyIdle { get; private set; }

		public bool SupportsMovingAtRuntime
		{
			get
			{
				return this.supportMovingAtRuntime;
			}
		}

		public bool hasPlayers
		{
			get
			{
				return this.localPlayerOn || this.remotePlayers.Count > 0;
			}
		}

		private void Awake()
		{
			base.transform.rotation = Quaternion.identity;
			this.scaleFactor = (base.transform.localScale.x + base.transform.localScale.y + base.transform.localScale.z) / 3f;
			this.SetIsIdle(true, false);
		}

		private void OnEnable()
		{
			VectorizedCustomRopeSimulation.Register(this);
		}

		private void OnDisable()
		{
			if (!this.isIdle)
			{
				this.SetIsIdle(true, true);
			}
			VectorizedCustomRopeSimulation.Unregister(this);
		}

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

		public Transform GetBone(int index)
		{
			if (index >= this.nodes.Length)
			{
				return this.nodes.Last<Transform>();
			}
			return this.nodes[index];
		}

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
				GorillaTagger.Instance.offlineVRRig.grabbedRopeIsLeft = xrNode == XRNode.LeftHand;
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

		public void DetachRemotePlayer(int playerId)
		{
			this.remotePlayers.Remove(playerId);
			this.RefreshAllBonesMass();
		}

		public void SetVelocity_RPC(int boneIndex, Vector3 velocity, bool wholeRope)
		{
			if (PhotonNetwork.InRoom)
			{
				base.photonView.RPC("SetVelocity", RpcTarget.All, new object[] { boneIndex, velocity, wholeRope });
				return;
			}
			this.SetVelocity(boneIndex, velocity, wholeRope, default(PhotonMessageInfo));
		}

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
			if (!(velocity).IsValid())
			{
				return;
			}
			velocity.x = Mathf.Clamp(velocity.x, -100f, 100f);
			velocity.y = Mathf.Clamp(velocity.y, -100f, 100f);
			velocity.z = Mathf.Clamp(velocity.z, -100f, 100f);
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

		public const float ropeBitGenOffset = 1f;

		[SerializeField]
		private GameObject prefabRopeBit;

		[SerializeField]
		private bool supportMovingAtRuntime;

		public Transform[] nodes = Array.Empty<Transform>();

		private Dictionary<int, int> remotePlayers = new Dictionary<int, int>();

		[NonSerialized]
		public float lastGrabTime;

		[SerializeField]
		private AudioSource ropeCreakSFX;

		public GorillaVelocityTracker velocityTracker;

		private bool localPlayerOn;

		private int localPlayerBoneIndex;

		private XRNode localPlayerXRNode;

		private const float MAX_VELOCITY_FOR_IDLE = 0.5f;

		private const float TIME_FOR_IDLE = 2f;

		private float potentialIdleTimer;

		[SerializeField]
		private int ropeLength = 8;

		[SerializeField]
		private GorillaRopeSwingSettings settings;

		[NonSerialized]
		public int ropeDataStartIndex;

		[NonSerialized]
		public int ropeDataIndexOffset;

		[SerializeField]
		private LayerMask wallLayerMask;

		private RaycastHit[] nodeHits = new RaycastHit[1];

		private float scaleFactor = 1f;

		private int lastNodeCheckIndex = 2;
	}
}
