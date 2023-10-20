﻿using System;
using GorillaExtensions;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000335 RID: 821
	public class EdibleWearable : MonoBehaviour
	{
		// Token: 0x060016C3 RID: 5827 RVA: 0x0007EC00 File Offset: 0x0007CE00
		protected void Awake()
		{
			this.edibleState = 0;
			this.previousEdibleState = 0;
			this.ownerRig = base.GetComponentInParent<VRRig>();
			this.isLocal = (this.ownerRig != null && this.ownerRig.isOfflineVRRig);
			this.isHandSlot = (this.wearablePackedStateSlot == VRRig.WearablePackedStateSlots.LeftHand || this.wearablePackedStateSlot == VRRig.WearablePackedStateSlots.RightHand);
			this.isLeftHand = (this.wearablePackedStateSlot == VRRig.WearablePackedStateSlots.LeftHand);
			this.stateBitsWriteInfo = VRRig.WearablePackedStatesBitWriteInfos[(int)this.wearablePackedStateSlot];
		}

		// Token: 0x060016C4 RID: 5828 RVA: 0x0007EC8C File Offset: 0x0007CE8C
		protected void OnEnable()
		{
			if (this.ownerRig == null)
			{
				Debug.LogError("EdibleWearable \"" + base.transform.GetPath() + "\": Deactivating because ownerRig is null.", this);
				base.gameObject.SetActive(false);
				return;
			}
			for (int i = 0; i < this.edibleStateInfos.Length; i++)
			{
				this.edibleStateInfos[i].gameObject.SetActive(i == this.edibleState);
			}
		}

		// Token: 0x060016C5 RID: 5829 RVA: 0x0007ED06 File Offset: 0x0007CF06
		protected virtual void LateUpdate()
		{
			if (this.isLocal)
			{
				this.LateUpdateLocal();
			}
			else
			{
				this.LateUpdateReplicated();
			}
			this.LateUpdateShared();
		}

		// Token: 0x060016C6 RID: 5830 RVA: 0x0007ED24 File Offset: 0x0007CF24
		protected virtual void LateUpdateLocal()
		{
			if (this.edibleState == this.edibleStateInfos.Length - 1)
			{
				if (Time.time > this.lastFullyEatenTime + this.respawnTime)
				{
					this.edibleState = 0;
					this.previousEdibleState = 0;
					this.OnEdibleHoldableStateChange();
				}
			}
			else if (Time.time > this.lastEatTime + this.biteCooldown)
			{
				Vector3 b = base.transform.TransformPoint(this.edibleBiteOffset);
				bool flag = false;
				float num = this.biteDistance * this.biteDistance;
				if (!GorillaParent.hasInstance)
				{
					return;
				}
				if ((GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform.TransformPoint(this.gorillaHeadMouthOffset) - b).sqrMagnitude < num)
				{
					flag = true;
				}
				foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
				{
					if (!flag)
					{
						if (vrrig.head == null)
						{
							break;
						}
						if (vrrig.head.rigTarget == null)
						{
							break;
						}
						if ((vrrig.head.rigTarget.transform.TransformPoint(this.gorillaHeadMouthOffset) - b).sqrMagnitude < num)
						{
							flag = true;
						}
					}
				}
				if (flag && !this.wasInBiteZoneLastFrame && this.edibleState < this.edibleStateInfos.Length)
				{
					this.edibleState++;
					this.lastEatTime = Time.time;
					this.lastFullyEatenTime = Time.time;
				}
				this.wasInBiteZoneLastFrame = flag;
			}
			this.ownerRig.WearablePackedStates = GTBitOps.WriteBits(this.ownerRig.WearablePackedStates, this.stateBitsWriteInfo, this.edibleState);
		}

		// Token: 0x060016C7 RID: 5831 RVA: 0x0007EEF0 File Offset: 0x0007D0F0
		protected virtual void LateUpdateReplicated()
		{
			this.edibleState = GTBitOps.ReadBits(this.ownerRig.WearablePackedStates, this.stateBitsWriteInfo.index, this.stateBitsWriteInfo.valueMask);
		}

		// Token: 0x060016C8 RID: 5832 RVA: 0x0007EF20 File Offset: 0x0007D120
		protected virtual void LateUpdateShared()
		{
			int num = this.edibleState;
			if (num != this.previousEdibleState)
			{
				this.OnEdibleHoldableStateChange();
			}
			this.previousEdibleState = num;
		}

		// Token: 0x060016C9 RID: 5833 RVA: 0x0007EF4C File Offset: 0x0007D14C
		protected virtual void OnEdibleHoldableStateChange()
		{
			if (this.previousEdibleState >= 0 && this.previousEdibleState < this.edibleStateInfos.Length)
			{
				this.edibleStateInfos[this.previousEdibleState].gameObject.SetActive(false);
			}
			if (this.edibleState >= 0 && this.edibleState < this.edibleStateInfos.Length)
			{
				EdibleWearable.EdibleStateInfo edibleStateInfo = this.edibleStateInfos[this.edibleState];
				edibleStateInfo.gameObject.SetActive(true);
				this.audioSource.PlayOneShot(edibleStateInfo.sound, this.volume);
			}
			float amplitude = GorillaTagger.Instance.tapHapticStrength / 4f;
			float fixedDeltaTime = Time.fixedDeltaTime;
			if (this.isLocal && this.isHandSlot)
			{
				GorillaTagger.Instance.StartVibration(this.isLeftHand, amplitude, fixedDeltaTime);
			}
		}

		// Token: 0x040018FA RID: 6394
		[Tooltip("Eating sounds are played through this AudioSource using PlayOneShot.")]
		public AudioSource audioSource;

		// Token: 0x040018FB RID: 6395
		[Tooltip("Volume each bite should play at.")]
		public float volume = 0.08f;

		// Token: 0x040018FC RID: 6396
		[Tooltip("The slot this cosmetic resides.")]
		public VRRig.WearablePackedStateSlots wearablePackedStateSlot = VRRig.WearablePackedStateSlots.LeftHand;

		// Token: 0x040018FD RID: 6397
		[Tooltip("Time between bites.")]
		public float biteCooldown = 1f;

		// Token: 0x040018FE RID: 6398
		[Tooltip("How long it takes to pop back to the uneaten state after being fully eaten.")]
		public float respawnTime = 7f;

		// Token: 0x040018FF RID: 6399
		[Tooltip("Distance from mouth to item required to trigger a bite.")]
		public float biteDistance = 0.5f;

		// Token: 0x04001900 RID: 6400
		[Tooltip("Offset from Gorilla's head to mouth.")]
		public Vector3 gorillaHeadMouthOffset = new Vector3(0f, 0.0208f, 0.171f);

		// Token: 0x04001901 RID: 6401
		[Tooltip("Offset from edible's transform to the bite point.")]
		public Vector3 edibleBiteOffset = new Vector3(0f, 0f, 0f);

		// Token: 0x04001902 RID: 6402
		public EdibleWearable.EdibleStateInfo[] edibleStateInfos;

		// Token: 0x04001903 RID: 6403
		private VRRig ownerRig;

		// Token: 0x04001904 RID: 6404
		private bool isLocal;

		// Token: 0x04001905 RID: 6405
		private bool isHandSlot;

		// Token: 0x04001906 RID: 6406
		private bool isLeftHand;

		// Token: 0x04001907 RID: 6407
		private GTBitOps.BitWriteInfo stateBitsWriteInfo;

		// Token: 0x04001908 RID: 6408
		private int edibleState;

		// Token: 0x04001909 RID: 6409
		private int previousEdibleState;

		// Token: 0x0400190A RID: 6410
		private float lastEatTime;

		// Token: 0x0400190B RID: 6411
		private float lastFullyEatenTime;

		// Token: 0x0400190C RID: 6412
		private bool wasInBiteZoneLastFrame;

		// Token: 0x02000515 RID: 1301
		[Serializable]
		public struct EdibleStateInfo
		{
			// Token: 0x04002142 RID: 8514
			[Tooltip("Will be activated when this stage is reached.")]
			public GameObject gameObject;

			// Token: 0x04002143 RID: 8515
			[Tooltip("Will be played when this stage is reached.")]
			public AudioClip sound;
		}
	}
}
