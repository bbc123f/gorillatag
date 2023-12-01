using System;
using UnityEngine;

public class SlingshotProjectileHitNotifier : MonoBehaviour
{
	public event SlingshotProjectileHitNotifier.ProjectileHitEvent OnProjectileHit;

	public event SlingshotProjectileHitNotifier.ProjectileHitEvent OnProjectileCollisionStay;

	public void InvokeHit(SlingshotProjectile projectile, Collision collision)
	{
		SlingshotProjectileHitNotifier.ProjectileHitEvent onProjectileHit = this.OnProjectileHit;
		if (onProjectileHit == null)
		{
			return;
		}
		onProjectileHit(projectile, collision);
	}

	public void InvokeCollisionStay(SlingshotProjectile projectile, Collision collision)
	{
		SlingshotProjectileHitNotifier.ProjectileHitEvent onProjectileCollisionStay = this.OnProjectileCollisionStay;
		if (onProjectileCollisionStay == null)
		{
			return;
		}
		onProjectileCollisionStay(projectile, collision);
	}

	private void OnDestroy()
	{
		this.OnProjectileHit = null;
		this.OnProjectileCollisionStay = null;
	}

	public delegate void ProjectileHitEvent(SlingshotProjectile projectile, Collision collision);
}
