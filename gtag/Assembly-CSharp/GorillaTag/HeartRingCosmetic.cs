using System;
using GorillaExtensions;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x020002FA RID: 762
	[DefaultExecutionOrder(1250)]
	public class HeartRingCosmetic : MonoBehaviour
	{
		// Token: 0x06001570 RID: 5488 RVA: 0x000769E9 File Offset: 0x00074BE9
		protected void Awake()
		{
			Application.quitting += delegate()
			{
				base.enabled = false;
			};
		}

		// Token: 0x06001571 RID: 5489 RVA: 0x000769FC File Offset: 0x00074BFC
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

		// Token: 0x06001572 RID: 5490 RVA: 0x00076B04 File Offset: 0x00074D04
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

		// Token: 0x04001775 RID: 6005
		public GameObject effects;

		// Token: 0x04001776 RID: 6006
		[SerializeField]
		private bool isHauntedVoiceChanger;

		// Token: 0x04001777 RID: 6007
		[SerializeField]
		private float hauntedVoicePitch = 0.75f;

		// Token: 0x04001778 RID: 6008
		[AssignInCorePrefab]
		public float effectActivationRadius = 0.15f;

		// Token: 0x04001779 RID: 6009
		private readonly Vector3 headToMouthOffset = new Vector3(0f, 0.0208f, 0.171f);

		// Token: 0x0400177A RID: 6010
		private VRRig ownerRig;

		// Token: 0x0400177B RID: 6011
		private Transform ownerHead;

		// Token: 0x0400177C RID: 6012
		private ParticleSystem particleSystem;

		// Token: 0x0400177D RID: 6013
		private AudioSource audioSource;

		// Token: 0x0400177E RID: 6014
		private float maxEmissionRate;

		// Token: 0x0400177F RID: 6015
		private float maxVolume;

		// Token: 0x04001780 RID: 6016
		private const float emissionFadeTime = 0.1f;

		// Token: 0x04001781 RID: 6017
		private const float volumeFadeTime = 2f;
	}
}
