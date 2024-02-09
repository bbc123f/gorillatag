using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaNetworking;
using UnityEngine;

namespace GorillaTag.Reactions
{
	public class ParticleCollisionWatcher : MonoBehaviour
	{
		protected void OnEnable()
		{
			if (GorillaComputer.instance == null)
			{
				Debug.LogError("ParticleCollisionWatcher: Disabling because GorillaComputer not found! Hierarchy path: " + base.transform.GetPath(), this);
				base.enabled = false;
				return;
			}
			if (this._prefabToSpawn == null)
			{
				Debug.LogError("ParticleCollisionWatcher: Disabling because Spawn Prefab not assigned! Hierarchy path: " + base.transform.GetPath(), this);
				base.enabled = false;
				return;
			}
			if (!this._isPrefabInPool)
			{
				if (this._prefabToSpawn.CompareTag("Untagged"))
				{
					Debug.LogError("ParticleCollisionWatcher: Disabling because Spawn Prefab has no tag! Hierarchy path: " + base.transform.GetPath(), this);
					base.enabled = false;
					return;
				}
				this._isPrefabInPool = ObjectPools.instance.DoesPoolExist(this._prefabToSpawn);
				if (!this._isPrefabInPool)
				{
					Debug.LogError("ParticleCollisionWatcher: Disabling because Spawn Prefab not in pool! Hierarchy path: " + base.transform.GetPath(), this);
					base.enabled = false;
					return;
				}
			}
			if (this._particleSystem == null)
			{
				this._particleSystem = base.GetComponent<ParticleSystem>();
			}
			if (this._particleSystem == null)
			{
				Debug.LogError("ParticleCollisionWatcher: Disabling because could not find ParticleSystem! Hierarchy path: " + base.transform.GetPath(), this);
				base.enabled = false;
				return;
			}
			if (this._collisionEvents == null)
			{
				this._collisionEvents = new List<ParticleCollisionEvent>(this._particleSystem.main.maxParticles);
			}
		}

		protected void OnParticleCollision(GameObject other)
		{
			if (this._maxParticleEventsRate <= 0f)
			{
				return;
			}
			long startupMillis = GorillaComputer.instance.startupMillis;
			if (startupMillis == 0L)
			{
				return;
			}
			double num = (double)startupMillis / 1000.0 + Time.realtimeSinceStartupAsDouble;
			if ((float)(num - this._lastCollisionTime) < 1f / this._maxParticleEventsRate)
			{
				return;
			}
			if (this._particleSystem.GetCollisionEvents(other, this._collisionEvents) <= 0)
			{
				return;
			}
			float x = base.transform.lossyScale.x;
			ParticleCollisionEvent particleCollisionEvent = this._collisionEvents[0];
			ObjectPools.instance.Instantiate(this._prefabToSpawn, particleCollisionEvent.intersection, Quaternion.LookRotation(this._collisionEvents[0].normal), x);
			this._lastCollisionTime = num;
		}

		[Tooltip("The defaults are numbers for the flamethrower hair dryer.")]
		private readonly float _maxParticleEventsRate = 2f;

		[Tooltip("Must be in the global object pool and have a tag.")]
		[SerializeField]
		private GameObject _prefabToSpawn;

		private ParticleSystem _particleSystem;

		private List<ParticleCollisionEvent> _collisionEvents;

		private bool _isPrefabInPool;

		private double _lastCollisionTime;
	}
}
