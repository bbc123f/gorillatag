using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTag.Reactions
{
	public class FireExtinguishReaction : MonoBehaviour
	{
		protected void Awake()
		{
			FireExtinguishReactionManager.Register(this);
		}

		protected void OnDestroy()
		{
			FireExtinguishReactionManager.Unregister(this);
		}

		protected void OnEnable()
		{
			FireExtinguishReactionManager.OnEnable(this);
		}

		protected void OnDisable()
		{
			FireExtinguishReactionManager.OnDisable(this);
		}

		[Header("Scene References")]
		[Tooltip("If not assigned it will try to auto assign to a component on the same GameObject.")]
		[SerializeField]
		internal Collider _collider;

		[Tooltip("If not assigned it will try to auto assign to a component on the same GameObject.")]
		[FormerlySerializedAs("_thermalSourceVolume")]
		[SerializeField]
		internal ThermalSourceVolume _thermalVolume;

		[SerializeField]
		internal ParticleSystem _particleSystem;

		[FormerlySerializedAs("_audioSource")]
		[SerializeField]
		internal AudioSource _loopingAudioSource;

		[Header("Asset References")]
		[SerializeField]
		internal SturdyAssetRef<AudioClip> _extinguishSound;

		[SerializeField]
		internal float _extinguishSoundVolume = 1f;

		[SerializeField]
		internal SturdyAssetRef<AudioClip> _igniteSound;

		[SerializeField]
		internal float _igniteSoundVolume = 1f;

		[Header("Values")]
		[SerializeField]
		internal bool _despawnOnExtinguish = true;

		[SerializeField]
		internal float _maxLifetime = 10f;

		[Tooltip("How long it should take to reheat to it's default temperature.")]
		[SerializeField]
		internal float _reheatSpeed = 1f;

		[Tooltip("If you completely extinguish the object, how long should it stay extinguished?")]
		[SerializeField]
		internal float _stayExtinguishedDuration = 1f;

		internal float _defaultTemperature;

		internal float _timeSinceExtinguish;

		internal float _timSinceDyingStart;

		internal float _timeAlive;

		internal float _defaultParticleEmissionRate;

		internal bool _isDying;
	}
}
