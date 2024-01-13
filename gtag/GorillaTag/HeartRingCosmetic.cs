using UnityEngine;

namespace GorillaTag;

public class HeartRingCosmetic : MonoBehaviour
{
	public GameObject effects;

	[AssignInCorePrefab]
	public float effectActivationRadius = 0.15f;

	private readonly Vector3 headToMouthOffset = new Vector3(0f, 0.0208f, 0.171f);

	private Transform ownerHead;

	private ParticleSystem particleSystem;

	private AudioSource audioSource;

	private float maxEmissionRate;

	private float maxVolume;

	private const float emissionFadeTime = 0.1f;

	private const float volumeFadeTime = 2f;

	protected void OnEnable()
	{
		particleSystem = effects.GetComponentInChildren<ParticleSystem>(includeInactive: true);
		audioSource = effects.GetComponentInChildren<AudioSource>(includeInactive: true);
		VRRig componentInParent = GetComponentInParent<VRRig>();
		bool flag2 = (base.enabled = componentInParent != null && componentInParent.head != null && componentInParent.head.rigTarget != null);
		effects.SetActive(flag2);
		if (flag2)
		{
			ownerHead = ((componentInParent != null) ? componentInParent.head.rigTarget.transform : base.transform);
			maxEmissionRate = particleSystem.emission.rateOverTime.constant;
			maxVolume = audioSource.volume;
		}
	}

	protected void LateUpdate()
	{
		Transform obj = base.transform;
		Vector3 position = obj.position;
		float x = obj.lossyScale.x;
		float num = effectActivationRadius * effectActivationRadius * x * x;
		bool flag = (ownerHead.TransformPoint(headToMouthOffset) - position).sqrMagnitude < num;
		ParticleSystem.EmissionModule emission = particleSystem.emission;
		emission.rateOverTime = Mathf.Lerp(emission.rateOverTime.constant, flag ? maxEmissionRate : 0f, Time.deltaTime / 0.1f);
		audioSource.volume = Mathf.Lerp(audioSource.volume, flag ? maxVolume : 0f, Time.deltaTime / 2f);
	}
}
