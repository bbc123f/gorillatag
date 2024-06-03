using System;
using UnityEngine;

public class FeatherDusterHoldable : MonoBehaviour
{
	protected void Awake()
	{
		this.timeSinceLastSound = this.soundCooldown;
		this.emissionModule = this.particleFx.emission;
		this.initialRateOverTime = this.emissionModule.rateOverTimeMultiplier;
	}

	protected void OnEnable()
	{
		this.lastWorldPos = base.transform.position;
		this.emissionModule.rateOverTimeMultiplier = 0f;
	}

	protected void Update()
	{
		this.timeSinceLastSound += Time.deltaTime;
		Transform transform = base.transform;
		Vector3 position = transform.position;
		float num = (position - this.lastWorldPos).magnitude / Time.deltaTime;
		this.emissionModule.rateOverTimeMultiplier = 0f;
		if (num >= this.collideMinSpeed && Physics.OverlapSphereNonAlloc(position, this.overlapSphereRadius * transform.localScale.x, this.colliderResult, this.collisionLayer) > 0)
		{
			this.emissionModule.rateOverTimeMultiplier = this.initialRateOverTime;
			if (this.timeSinceLastSound >= this.soundCooldown)
			{
				this.soundBankPlayer.Play(null, null);
				this.timeSinceLastSound = 0f;
			}
		}
		this.lastWorldPos = position;
	}

	public FeatherDusterHoldable()
	{
	}

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
}
