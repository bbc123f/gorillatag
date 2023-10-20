using System;
using GorillaExtensions;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x020002FC RID: 764
	[DefaultExecutionOrder(1250)]
	public class HeartRingCosmetic : MonoBehaviour
	{
		// Token: 0x06001579 RID: 5497 RVA: 0x00076ED1 File Offset: 0x000750D1
		protected void Awake()
		{
			Application.quitting += delegate()
			{
				base.enabled = false;
			};
		}

		// Token: 0x0600157A RID: 5498 RVA: 0x00076EE4 File Offset: 0x000750E4
		protected void OnEnable()
		{
			this.particleSystem = this.effects.GetComponentInChildren<ParticleSystem>(true);
			this.audioSource = this.effects.GetComponentInChildren<AudioSource>(true);
			this.ownerRig = base.GetComponentInParent<VRRig>();
			bool flag = this.ownerRig != null && this.ownerRig.head != null && this.ownerRig.head.rigTarget != null;
			base.enabled = flag;
			this.effects.SetActive(flag);
			if (!flag)
			{
				Debug.LogError("Disabling HeartRingCosmetic. Could not find owner head. Scene path: " + base.transform.GetPath(), this);
				return;
			}
			this.ownerHead = ((this.ownerRig != null) ? this.ownerRig.head.rigTarget.transform : base.transform);
			this.maxEmissionRate = this.particleSystem.emission.rateOverTime.constant;
			this.maxVolume = this.audioSource.volume;
		}

		// Token: 0x0600157B RID: 5499 RVA: 0x00076FEC File Offset: 0x000751EC
		protected void LateUpdate()
		{
			Transform transform = base.transform;
			Vector3 position = transform.position;
			float x = transform.lossyScale.x;
			float num = this.effectActivationRadius * this.effectActivationRadius * x * x;
			bool flag = (this.ownerHead.TransformPoint(this.headToMouthOffset) - position).sqrMagnitude < num;
			ParticleSystem.EmissionModule emission = this.particleSystem.emission;
			emission.rateOverTime = Mathf.Lerp(emission.rateOverTime.constant, flag ? this.maxEmissionRate : 0f, Time.deltaTime / 0.1f);
			this.audioSource.volume = Mathf.Lerp(this.audioSource.volume, flag ? this.maxVolume : 0f, Time.deltaTime / 2f);
			this.ownerRig.UsingHauntedRing = (this.isHauntedVoiceChanger && flag);
			if (this.ownerRig.UsingHauntedRing)
			{
				this.ownerRig.HauntedRingVoicePitch = this.hauntedVoicePitch;
			}
		}

		// Token: 0x04001782 RID: 6018
		public GameObject effects;

		// Token: 0x04001783 RID: 6019
		[SerializeField]
		private bool isHauntedVoiceChanger;

		// Token: 0x04001784 RID: 6020
		[SerializeField]
		private float hauntedVoicePitch = 0.75f;

		// Token: 0x04001785 RID: 6021
		[AssignInCorePrefab]
		public float effectActivationRadius = 0.15f;

		// Token: 0x04001786 RID: 6022
		private readonly Vector3 headToMouthOffset = new Vector3(0f, 0.0208f, 0.171f);

		// Token: 0x04001787 RID: 6023
		private VRRig ownerRig;

		// Token: 0x04001788 RID: 6024
		private Transform ownerHead;

		// Token: 0x04001789 RID: 6025
		private ParticleSystem particleSystem;

		// Token: 0x0400178A RID: 6026
		private AudioSource audioSource;

		// Token: 0x0400178B RID: 6027
		private float maxEmissionRate;

		// Token: 0x0400178C RID: 6028
		private float maxVolume;

		// Token: 0x0400178D RID: 6029
		private const float emissionFadeTime = 0.1f;

		// Token: 0x0400178E RID: 6030
		private const float volumeFadeTime = 2f;
	}
}
