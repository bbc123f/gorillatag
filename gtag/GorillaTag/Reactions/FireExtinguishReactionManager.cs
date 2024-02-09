using System;
using System.Collections.Generic;
using GorillaTag.Audio;
using UnityEngine;

namespace GorillaTag.Reactions
{
	public class FireExtinguishReactionManager : ITickSystemPost
	{
		[OnEnterPlay_SetNull]
		internal static FireExtinguishReactionManager instance { get; private set; }

		[OnEnterPlay_Set(false)]
		internal static bool hasInstance { get; private set; }

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void Initialize()
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			if (FireExtinguishReactionManager.hasInstance)
			{
				return;
			}
			FireExtinguishReactionManager.instance = new FireExtinguishReactionManager();
			FireExtinguishReactionManager.hasInstance = true;
			TickSystem<object>.AddPostTickCallback(FireExtinguishReactionManager.instance);
		}

		internal static void Register(FireExtinguishReaction reactable)
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			int instanceID = reactable.gameObject.GetInstanceID();
			if (!FireExtinguishReactionManager._kGObj_to_extinguishReaction.TryAdd(instanceID, reactable))
			{
				if (reactable == null)
				{
					Debug.LogError("FireExtinguishReactionManager: You tried to register null!", reactable);
					return;
				}
				Debug.LogError("FireExtinguishReactionManager: \"" + reactable.name + "\" was attempted to be registered more than once!", reactable);
			}
			reactable.GetComponentAndSetFieldIfNullElseLogAndDisable(ref reactable._collider, "_collider", "Collider", "Disabling.", "Register");
			reactable.GetComponentAndSetFieldIfNullElseLogAndDisable(ref reactable._thermalVolume, "_thermalVolume", "ThermalSourceVolume", "Disabling.", "Register");
			reactable.GetComponentAndSetFieldIfNullElseLogAndDisable(ref reactable._particleSystem, "_particleSystem", "ParticleSystem", "Disabling.", "Register");
			reactable.GetComponentAndSetFieldIfNullElseLogAndDisable(ref reactable._loopingAudioSource, "_loopingAudioSource", "AudioSource", "Disabling.", "Register");
			reactable.DisableIfNull(reactable._extinguishSound.obj, "_extinguishSound", "AudioClip", "Register");
			reactable.DisableIfNull(reactable._igniteSound.obj, "_igniteSound", "AudioClip", "Register");
			reactable._defaultTemperature = reactable._thermalVolume.celsius;
			reactable._timeSinceExtinguish = -reactable._stayExtinguishedDuration;
			reactable._defaultParticleEmissionRate = reactable._particleSystem.emission.rateOverTime.constant;
		}

		internal static void Unregister(FireExtinguishReaction reactable)
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			int instanceID = reactable.gameObject.GetInstanceID();
			FireExtinguishReactionManager._kGObj_to_extinguishReaction.Remove(instanceID);
		}

		internal static void OnEnable(FireExtinguishReaction r)
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			r._timeSinceExtinguish = -r._stayExtinguishedDuration;
			r._timSinceDyingStart = 0f;
			r._isDying = false;
			r._timeAlive = 0f;
			FireExtinguishReactionManager._kEnabledReactions.Add(r);
			if (Time.realtimeSinceStartup > 10f)
			{
				GTAudioOneShot.Play(r._igniteSound, r.transform.position, r._igniteSoundVolume, 1f);
			}
			if (FireExtinguishReactionManager._maxAudioSources > FireExtinguishReactionManager._activeAudioSources)
			{
				FireExtinguishReactionManager._activeAudioSources++;
				r._loopingAudioSource.enabled = true;
				return;
			}
			r._loopingAudioSource.enabled = false;
		}

		internal static void OnDisable(FireExtinguishReaction reactable)
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			reactable._timeSinceExtinguish = -reactable._stayExtinguishedDuration;
			FireExtinguishReactionManager._kEnabledReactions.Remove(reactable);
			FireExtinguishReactionManager._activeAudioSources = Mathf.Min(FireExtinguishReactionManager._activeAudioSources - (reactable._loopingAudioSource.enabled ? 1 : 0), 0);
		}

		internal static void ApplyThermalDamage(int gObjInstId, float damage)
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			FireExtinguishReaction fireExtinguishReaction;
			if (!FireExtinguishReactionManager._kGObj_to_extinguishReaction.TryGetValue(gObjInstId, out fireExtinguishReaction))
			{
				return;
			}
			float num = fireExtinguishReaction._thermalVolume.celsius - damage;
			if (num <= 0f && Math.Abs(num - fireExtinguishReaction._thermalVolume.celsius) > 0.001f)
			{
				fireExtinguishReaction._thermalVolume.celsius = Mathf.Max(num, 0f);
				fireExtinguishReaction._timeSinceExtinguish = 0f;
				GTAudioOneShot.Play(fireExtinguishReaction._extinguishSound, fireExtinguishReaction.transform.position, fireExtinguishReaction._extinguishSoundVolume, 1f);
			}
		}

		bool ITickSystemPost.PostTickRunning { get; set; }

		void ITickSystemPost.PostTick()
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			for (int i = 0; i < FireExtinguishReactionManager._kEnabledReactions.Count; i++)
			{
				FireExtinguishReaction fireExtinguishReaction = FireExtinguishReactionManager._kEnabledReactions[i];
				fireExtinguishReaction._timeAlive += Time.unscaledDeltaTime;
				if (fireExtinguishReaction._isDying)
				{
					fireExtinguishReaction._timSinceDyingStart += Time.unscaledDeltaTime;
					if (fireExtinguishReaction._timSinceDyingStart >= fireExtinguishReaction._extinguishSound.obj.length)
					{
						FireExtinguishReactionManager._kToDestroy.Add(fireExtinguishReaction);
					}
				}
				if (fireExtinguishReaction._timeAlive > fireExtinguishReaction._maxLifetime)
				{
					fireExtinguishReaction._isDying = true;
					fireExtinguishReaction._timSinceDyingStart = 0f;
					GTAudioOneShot.Play(fireExtinguishReaction._extinguishSound, fireExtinguishReaction.transform.position, fireExtinguishReaction._extinguishSoundVolume, 1f);
				}
				bool flag = fireExtinguishReaction._timeSinceExtinguish < fireExtinguishReaction._stayExtinguishedDuration;
				fireExtinguishReaction._timeSinceExtinguish += Time.unscaledDeltaTime;
				bool flag2 = fireExtinguishReaction._timeSinceExtinguish < fireExtinguishReaction._stayExtinguishedDuration;
				if (flag != flag2)
				{
					if (flag2)
					{
						if (fireExtinguishReaction._despawnOnExtinguish)
						{
							fireExtinguishReaction._isDying = true;
							fireExtinguishReaction._timSinceDyingStart = 0f;
						}
						GTAudioOneShot.Play(fireExtinguishReaction._extinguishSound, fireExtinguishReaction.transform.position, fireExtinguishReaction._extinguishSoundVolume, 1f);
					}
					else
					{
						GTAudioOneShot.Play(fireExtinguishReaction._igniteSound, fireExtinguishReaction.transform.position, fireExtinguishReaction._igniteSoundVolume, 1f);
					}
				}
				float num = fireExtinguishReaction._thermalVolume.celsius + fireExtinguishReaction._reheatSpeed * Time.unscaledDeltaTime;
				fireExtinguishReaction._thermalVolume.celsius = ((num > fireExtinguishReaction._defaultTemperature) ? fireExtinguishReaction._defaultTemperature : num);
				float num2 = fireExtinguishReaction._thermalVolume.celsius / fireExtinguishReaction._defaultTemperature;
				fireExtinguishReaction._particleSystem.emission.rateOverTime = fireExtinguishReaction._defaultParticleEmissionRate * (fireExtinguishReaction._thermalVolume.celsius / fireExtinguishReaction._defaultTemperature);
				fireExtinguishReaction._loopingAudioSource.volume = num2;
			}
			foreach (FireExtinguishReaction fireExtinguishReaction2 in FireExtinguishReactionManager._kToDestroy)
			{
				ObjectPools.instance.Destroy(fireExtinguishReaction2.gameObject);
			}
		}

		[NonSerialized]
		private static readonly Dictionary<int, FireExtinguishReaction> _kGObj_to_extinguishReaction = new Dictionary<int, FireExtinguishReaction>(256);

		private static readonly List<FireExtinguishReaction> _kEnabledReactions = new List<FireExtinguishReaction>(256);

		private static readonly List<FireExtinguishReaction> _kToDestroy = new List<FireExtinguishReaction>(256);

		private static int _maxAudioSources = 8;

		private static int _activeAudioSources = 0;
	}
}
