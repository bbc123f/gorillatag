using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x0200029E RID: 670
	public class OldGorillaRopeSwing : MonoBehaviourPun
	{
		// Token: 0x17000106 RID: 262
		// (get) Token: 0x06001171 RID: 4465 RVA: 0x000624FD File Offset: 0x000606FD
		// (set) Token: 0x06001172 RID: 4466 RVA: 0x00062505 File Offset: 0x00060705
		public bool isIdle { get; private set; }

		// Token: 0x06001173 RID: 4467 RVA: 0x0006250E File Offset: 0x0006070E
		private void Awake()
		{
			this.SetIsIdle(true);
		}

		// Token: 0x06001174 RID: 4468 RVA: 0x00062517 File Offset: 0x00060717
		private void OnDisable()
		{
			if (!this.isIdle)
			{
				this.SetIsIdle(true);
			}
		}

		// Token: 0x06001175 RID: 4469 RVA: 0x00062528 File Offset: 0x00060728
		private void Update()
		{
			if (this.localPlayerOn && this.localGrabbedRigid)
			{
				float magnitude = this.localGrabbedRigid.velocity.magnitude;
				if (magnitude > 2.5f && !this.ropeCreakSFX.isPlaying && Mathf.RoundToInt(Time.time) % 5 == 0)
				{
					this.ropeCreakSFX.Play();
				}
				float num = MathUtils.Linear(magnitude, 0f, 10f, -0.07f, 0.5f);
				if (num > 0f)
				{
					GorillaTagger.Instance.DoVibration(this.localPlayerXRNode, num, Time.deltaTime);
				}
			}
			if (!this.isIdle)
			{
				if (!this.localPlayerOn && this.remotePlayers.Count == 0)
				{
					foreach (Rigidbody rigidbody in this.bones)
					{
						float magnitude2 = rigidbody.velocity.magnitude;
						float num2 = Time.deltaTime * this.settings.frictionWhenNotHeld;
						if (num2 < magnitude2 - 0.1f)
						{
							rigidbody.velocity = Vector3.MoveTowards(rigidbody.velocity, Vector3.zero, num2);
						}
					}
				}
				bool flag = false;
				for (int j = 0; j < this.bones.Length; j++)
				{
					if (this.bones[j].velocity.magnitude > 0.1f)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					this.potentialIdleTimer += Time.deltaTime;
				}
				else
				{
					this.potentialIdleTimer = 0f;
				}
				if (this.potentialIdleTimer >= 2f)
				{
					this.SetIsIdle(true);
					this.potentialIdleTimer = 0f;
				}
			}
		}

		// Token: 0x06001176 RID: 4470 RVA: 0x000626D4 File Offset: 0x000608D4
		private void SetIsIdle(bool idle)
		{
			this.isIdle = idle;
			this.ToggleIsKinematic(idle);
			if (idle)
			{
				for (int i = 0; i < this.bones.Length; i++)
				{
					this.bones[i].velocity = Vector3.zero;
					this.bones[i].angularVelocity = Vector3.zero;
					this.bones[i].transform.localRotation = Quaternion.identity;
				}
			}
		}

		// Token: 0x06001177 RID: 4471 RVA: 0x00062740 File Offset: 0x00060940
		private void ToggleIsKinematic(bool kinematic)
		{
			for (int i = 0; i < this.bones.Length; i++)
			{
				this.bones[i].isKinematic = kinematic;
				if (kinematic)
				{
					this.bones[i].interpolation = RigidbodyInterpolation.None;
				}
				else
				{
					this.bones[i].interpolation = RigidbodyInterpolation.Interpolate;
				}
			}
		}

		// Token: 0x06001178 RID: 4472 RVA: 0x0006278F File Offset: 0x0006098F
		public Rigidbody GetBone(int index)
		{
			if (index >= this.bones.Length)
			{
				return this.bones.Last<Rigidbody>();
			}
			return this.bones[index];
		}

		// Token: 0x06001179 RID: 4473 RVA: 0x000627B0 File Offset: 0x000609B0
		public int GetBoneIndex(Rigidbody r)
		{
			for (int i = 0; i < this.bones.Length; i++)
			{
				if (this.bones[i] == r)
				{
					return i;
				}
			}
			return this.bones.Length - 1;
		}

		// Token: 0x0600117A RID: 4474 RVA: 0x000627EC File Offset: 0x000609EC
		public void AttachLocalPlayer(XRNode xrNode, Rigidbody rigid, Vector3 offset, Vector3 velocity)
		{
			int boneIndex = this.GetBoneIndex(rigid);
			velocity *= this.settings.inheritVelocityMultiplier;
			if (GorillaTagger.hasInstance && GorillaTagger.Instance.offlineVRRig)
			{
				GorillaTagger.Instance.offlineVRRig.grabbedRopeIndex = base.photonView.ViewID;
				GorillaTagger.Instance.offlineVRRig.grabbedRopeBoneIndex = boneIndex;
				GorillaTagger.Instance.offlineVRRig.grabbedRopeIsLeft = (xrNode == XRNode.LeftHand);
				GorillaTagger.Instance.offlineVRRig.grabbedRopeOffset = offset;
			}
			List<Vector3> list = new List<Vector3>();
			List<Vector3> list2 = new List<Vector3>();
			if (this.remotePlayers.Count <= 0)
			{
				foreach (Rigidbody rigidbody in this.bones)
				{
					list.Add(rigidbody.transform.localEulerAngles);
					list2.Add(rigidbody.velocity);
				}
			}
			if (Time.time - this.lastGrabTime > 1f && (this.remotePlayers.Count == 0 || velocity.magnitude > 2f))
			{
				this.SetVelocity_RPC(boneIndex, velocity, true, list.ToArray(), list2.ToArray());
			}
			this.lastGrabTime = Time.time;
			this.ropeCreakSFX.transform.parent = this.GetBone(Math.Max(0, boneIndex - 2)).transform;
			this.ropeCreakSFX.transform.localPosition = Vector3.zero;
			this.localPlayerOn = true;
			this.localPlayerXRNode = xrNode;
			this.localGrabbedRigid = rigid;
		}

		// Token: 0x0600117B RID: 4475 RVA: 0x0006296F File Offset: 0x00060B6F
		public void DetachLocalPlayer()
		{
			if (GorillaTagger.hasInstance && GorillaTagger.Instance.offlineVRRig)
			{
				GorillaTagger.Instance.offlineVRRig.grabbedRopeIndex = -1;
			}
			this.localPlayerOn = false;
			this.localGrabbedRigid = null;
		}

		// Token: 0x0600117C RID: 4476 RVA: 0x000629A8 File Offset: 0x00060BA8
		public bool AttachRemotePlayer(int playerId, int boneIndex, Transform offsetTransform, Vector3 offset)
		{
			Rigidbody bone = this.GetBone(boneIndex);
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
			return true;
		}

		// Token: 0x0600117D RID: 4477 RVA: 0x00062A0F File Offset: 0x00060C0F
		public void DetachRemotePlayer(int playerId)
		{
			this.remotePlayers.Remove(playerId);
		}

		// Token: 0x0600117E RID: 4478 RVA: 0x00062A20 File Offset: 0x00060C20
		public void SetVelocity_RPC(int boneIndex, Vector3 velocity, bool wholeRope = true, Vector3[] ropeRotations = null, Vector3[] ropeVelocities = null)
		{
			if (PhotonNetwork.InRoom)
			{
				base.photonView.RPC("SetVelocity", RpcTarget.All, new object[]
				{
					boneIndex,
					velocity,
					wholeRope,
					ropeRotations,
					ropeVelocities
				});
				return;
			}
			this.SetVelocity(boneIndex, velocity, wholeRope, ropeRotations, ropeVelocities);
		}

		// Token: 0x0600117F RID: 4479 RVA: 0x00062A80 File Offset: 0x00060C80
		[PunRPC]
		public void SetVelocity(int boneIndex, Vector3 velocity, bool wholeRope = true, Vector3[] ropeRotations = null, Vector3[] ropeVelocities = null)
		{
			this.SetIsIdle(false);
			if (ropeRotations != null && ropeVelocities != null && ropeRotations.Length != 0)
			{
				this.ToggleIsKinematic(true);
				for (int i = 0; i < ropeRotations.Length; i++)
				{
					if (i != 0)
					{
						this.bones[i].transform.localRotation = Quaternion.Euler(ropeRotations[i]);
						this.bones[i].velocity = ropeVelocities[i];
					}
				}
				this.ToggleIsKinematic(false);
			}
			Rigidbody bone = this.GetBone(boneIndex);
			if (bone)
			{
				if (wholeRope)
				{
					int num = 0;
					float maxLength = Mathf.Min(velocity.magnitude, 15f);
					foreach (Rigidbody rigidbody in this.bones)
					{
						Vector3 vector = velocity / (float)boneIndex * (float)num;
						vector = Vector3.ClampMagnitude(vector, maxLength);
						rigidbody.velocity = vector;
						num++;
					}
					return;
				}
				bone.velocity = velocity;
			}
		}

		// Token: 0x04001412 RID: 5138
		public const float kPlayerMass = 0.8f;

		// Token: 0x04001413 RID: 5139
		public const float ropeBitGenOffset = 1f;

		// Token: 0x04001414 RID: 5140
		public const float MAX_ROPE_SPEED = 15f;

		// Token: 0x04001415 RID: 5141
		[SerializeField]
		private GameObject prefabRopeBit;

		// Token: 0x04001416 RID: 5142
		public Rigidbody[] bones = Array.Empty<Rigidbody>();

		// Token: 0x04001417 RID: 5143
		private Dictionary<int, int> remotePlayers = new Dictionary<int, int>();

		// Token: 0x04001418 RID: 5144
		[NonSerialized]
		public float lastGrabTime;

		// Token: 0x04001419 RID: 5145
		[SerializeField]
		private AudioSource ropeCreakSFX;

		// Token: 0x0400141A RID: 5146
		private bool localPlayerOn;

		// Token: 0x0400141B RID: 5147
		private XRNode localPlayerXRNode;

		// Token: 0x0400141C RID: 5148
		private Rigidbody localGrabbedRigid;

		// Token: 0x0400141D RID: 5149
		private const float MAX_VELOCITY_FOR_IDLE = 0.1f;

		// Token: 0x0400141E RID: 5150
		private const float TIME_FOR_IDLE = 2f;

		// Token: 0x04001420 RID: 5152
		private float potentialIdleTimer;

		// Token: 0x04001421 RID: 5153
		[Header("Config")]
		[SerializeField]
		private int ropeLength = 8;

		// Token: 0x04001422 RID: 5154
		[SerializeField]
		private GorillaRopeSwingSettings settings;
	}
}
