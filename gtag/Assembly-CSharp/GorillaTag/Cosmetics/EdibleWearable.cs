using System;
using GorillaExtensions;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000333 RID: 819
	public class EdibleWearable : MonoBehaviour
	{
		// Token: 0x060016BA RID: 5818 RVA: 0x0007E718 File Offset: 0x0007C918
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

		// Token: 0x060016BB RID: 5819 RVA: 0x0007E7A4 File Offset: 0x0007C9A4
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

		// Token: 0x060016BC RID: 5820 RVA: 0x0007E81E File Offset: 0x0007CA1E
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

		// Token: 0x060016BD RID: 5821 RVA: 0x0007E83C File Offset: 0x0007CA3C
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

		// Token: 0x060016BE RID: 5822 RVA: 0x0007EA08 File Offset: 0x0007CC08
		protected virtual void LateUpdateReplicated()
		{
			this.edibleState = GTBitOps.ReadBits(this.ownerRig.WearablePackedStates, this.stateBitsWriteInfo.index, this.stateBitsWriteInfo.valueMask);
		}

		// Token: 0x060016BF RID: 5823 RVA: 0x0007EA38 File Offset: 0x0007CC38
		protected virtual void LateUpdateShared()
		{
			int num = this.edibleState;
			if (num != this.previousEdibleState)
			{
				this.OnEdibleHoldableStateChange();
			}
			this.previousEdibleState = num;
		}

		// Token: 0x060016C0 RID: 5824 RVA: 0x0007EA64 File Offset: 0x0007CC64
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

		// Token: 0x040018ED RID: 6381
		[Tooltip("Eating sounds are played through this AudioSource using PlayOneShot.")]
		public AudioSource audioSource;

		// Token: 0x040018EE RID: 6382
		[Tooltip("Volume each bite should play at.")]
		public float volume = 0.08f;

		// Token: 0x040018EF RID: 6383
		[Tooltip("The slot this cosmetic resides.")]
		public VRRig.WearablePackedStateSlots wearablePackedStateSlot = VRRig.WearablePackedStateSlots.LeftHand;

		// Token: 0x040018F0 RID: 6384
		[Tooltip("Time between bites.")]
		public float biteCooldown = 1f;

		// Token: 0x040018F1 RID: 6385
		[Tooltip("How long it takes to pop back to the uneaten state after being fully eaten.")]
		public float respawnTime = 7f;

		// Token: 0x040018F2 RID: 6386
		[Tooltip("Distance from mouth to item required to trigger a bite.")]
		public float biteDistance = 0.5f;

		// Token: 0x040018F3 RID: 6387
		[Tooltip("Offset from Gorilla's head to mouth.")]
		public Vector3 gorillaHeadMouthOffset = new Vector3(0f, 0.0208f, 0.171f);

		// Token: 0x040018F4 RID: 6388
		[Tooltip("Offset from edible's transform to the bite point.")]
		public Vector3 edibleBiteOffset = new Vector3(0f, 0f, 0f);

		// Token: 0x040018F5 RID: 6389
		public EdibleWearable.EdibleStateInfo[] edibleStateInfos;

		// Token: 0x040018F6 RID: 6390
		private VRRig ownerRig;

		// Token: 0x040018F7 RID: 6391
		private bool isLocal;

		// Token: 0x040018F8 RID: 6392
		private bool isHandSlot;

		// Token: 0x040018F9 RID: 6393
		private bool isLeftHand;

		// Token: 0x040018FA RID: 6394
		private GTBitOps.BitWriteInfo stateBitsWriteInfo;

		// Token: 0x040018FB RID: 6395
		private int edibleState;

		// Token: 0x040018FC RID: 6396
		private int previousEdibleState;

		// Token: 0x040018FD RID: 6397
		private float lastEatTime;

		// Token: 0x040018FE RID: 6398
		private float lastFullyEatenTime;

		// Token: 0x040018FF RID: 6399
		private bool wasInBiteZoneLastFrame;

		// Token: 0x02000513 RID: 1299
		[Serializable]
		public struct EdibleStateInfo
		{
			// Token: 0x04002135 RID: 8501
			[Tooltip("Will be activated when this stage is reached.")]
			public GameObject gameObject;

			// Token: 0x04002136 RID: 8502
			[Tooltip("Will be played when this stage is reached.")]
			public AudioClip sound;
		}
	}
}
