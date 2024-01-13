using UnityEngine;

public class FeatherDusterHoldable : MonoBehaviour
{
	public LayerMask collisionLayer;

	public float overlapSphereRadius = 0.08f;

	[Tooltip("Collision is not tested until this speed requirement is met.")]
	private float collideMinSpeed = 1f;

	public ParticleSystem particleFx;

	public SoundBankPlayer soundBankPlayer;

	private float soundCooldown = 0.8f;

	private ParticleSystem.EmissionModule emissionModule;

	private float initialRateOverTime;

	private float timeSinceLastSound;

	private Vector3 lastWorldPos;

	private Collider[] colliderResult = new Collider[1];

	protected void Awake()
	{
		timeSinceLastSound = soundCooldown;
		emissionModule = particleFx.emission;
		initialRateOverTime = emissionModule.rateOverTimeMultiplier;
	}

	protected void OnEnable()
	{
		lastWorldPos = base.transform.position;
		emissionModule.rateOverTimeMultiplier = 0f;
	}

	protected void Update()
	{
		timeSinceLastSound += Time.deltaTime;
		Transform transform = base.transform;
		Vector3 position = transform.position;
		float num = (position - lastWorldPos).magnitude / Time.deltaTime;
		emissionModule.rateOverTimeMultiplier = 0f;
		if (num >= collideMinSpeed && Physics.OverlapSphereNonAlloc(position, overlapSphereRadius * transform.localScale.x, colliderResult, collisionLayer) > 0)
		{
			emissionModule.rateOverTimeMultiplier = initialRateOverTime;
			if (timeSinceLastSound >= soundCooldown)
			{
				soundBankPlayer.Play();
				timeSinceLastSound = 0f;
			}
		}
		lastWorldPos = position;
	}
}
