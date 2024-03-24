using System;
using System.Runtime.CompilerServices;
using System.Threading;
using GorillaTag.GuidedRefs;
using UnityEngine;

public class SlingshotProjectileHitNotifier : BaseGuidedRefTargetMono
{
	public event SlingshotProjectileHitNotifier.ProjectileHitEvent OnProjectileHit
	{
		[CompilerGenerated]
		add
		{
			SlingshotProjectileHitNotifier.ProjectileHitEvent projectileHitEvent = this.OnProjectileHit;
			SlingshotProjectileHitNotifier.ProjectileHitEvent projectileHitEvent2;
			do
			{
				projectileHitEvent2 = projectileHitEvent;
				SlingshotProjectileHitNotifier.ProjectileHitEvent projectileHitEvent3 = (SlingshotProjectileHitNotifier.ProjectileHitEvent)Delegate.Combine(projectileHitEvent2, value);
				projectileHitEvent = Interlocked.CompareExchange<SlingshotProjectileHitNotifier.ProjectileHitEvent>(ref this.OnProjectileHit, projectileHitEvent3, projectileHitEvent2);
			}
			while (projectileHitEvent != projectileHitEvent2);
		}
		[CompilerGenerated]
		remove
		{
			SlingshotProjectileHitNotifier.ProjectileHitEvent projectileHitEvent = this.OnProjectileHit;
			SlingshotProjectileHitNotifier.ProjectileHitEvent projectileHitEvent2;
			do
			{
				projectileHitEvent2 = projectileHitEvent;
				SlingshotProjectileHitNotifier.ProjectileHitEvent projectileHitEvent3 = (SlingshotProjectileHitNotifier.ProjectileHitEvent)Delegate.Remove(projectileHitEvent2, value);
				projectileHitEvent = Interlocked.CompareExchange<SlingshotProjectileHitNotifier.ProjectileHitEvent>(ref this.OnProjectileHit, projectileHitEvent3, projectileHitEvent2);
			}
			while (projectileHitEvent != projectileHitEvent2);
		}
	}

	public event SlingshotProjectileHitNotifier.ProjectileHitEvent OnProjectileCollisionStay
	{
		[CompilerGenerated]
		add
		{
			SlingshotProjectileHitNotifier.ProjectileHitEvent projectileHitEvent = this.OnProjectileCollisionStay;
			SlingshotProjectileHitNotifier.ProjectileHitEvent projectileHitEvent2;
			do
			{
				projectileHitEvent2 = projectileHitEvent;
				SlingshotProjectileHitNotifier.ProjectileHitEvent projectileHitEvent3 = (SlingshotProjectileHitNotifier.ProjectileHitEvent)Delegate.Combine(projectileHitEvent2, value);
				projectileHitEvent = Interlocked.CompareExchange<SlingshotProjectileHitNotifier.ProjectileHitEvent>(ref this.OnProjectileCollisionStay, projectileHitEvent3, projectileHitEvent2);
			}
			while (projectileHitEvent != projectileHitEvent2);
		}
		[CompilerGenerated]
		remove
		{
			SlingshotProjectileHitNotifier.ProjectileHitEvent projectileHitEvent = this.OnProjectileCollisionStay;
			SlingshotProjectileHitNotifier.ProjectileHitEvent projectileHitEvent2;
			do
			{
				projectileHitEvent2 = projectileHitEvent;
				SlingshotProjectileHitNotifier.ProjectileHitEvent projectileHitEvent3 = (SlingshotProjectileHitNotifier.ProjectileHitEvent)Delegate.Remove(projectileHitEvent2, value);
				projectileHitEvent = Interlocked.CompareExchange<SlingshotProjectileHitNotifier.ProjectileHitEvent>(ref this.OnProjectileCollisionStay, projectileHitEvent3, projectileHitEvent2);
			}
			while (projectileHitEvent != projectileHitEvent2);
		}
	}

	public event SlingshotProjectileHitNotifier.ProjectileTriggerEvent OnProjectileTriggerEnter
	{
		[CompilerGenerated]
		add
		{
			SlingshotProjectileHitNotifier.ProjectileTriggerEvent projectileTriggerEvent = this.OnProjectileTriggerEnter;
			SlingshotProjectileHitNotifier.ProjectileTriggerEvent projectileTriggerEvent2;
			do
			{
				projectileTriggerEvent2 = projectileTriggerEvent;
				SlingshotProjectileHitNotifier.ProjectileTriggerEvent projectileTriggerEvent3 = (SlingshotProjectileHitNotifier.ProjectileTriggerEvent)Delegate.Combine(projectileTriggerEvent2, value);
				projectileTriggerEvent = Interlocked.CompareExchange<SlingshotProjectileHitNotifier.ProjectileTriggerEvent>(ref this.OnProjectileTriggerEnter, projectileTriggerEvent3, projectileTriggerEvent2);
			}
			while (projectileTriggerEvent != projectileTriggerEvent2);
		}
		[CompilerGenerated]
		remove
		{
			SlingshotProjectileHitNotifier.ProjectileTriggerEvent projectileTriggerEvent = this.OnProjectileTriggerEnter;
			SlingshotProjectileHitNotifier.ProjectileTriggerEvent projectileTriggerEvent2;
			do
			{
				projectileTriggerEvent2 = projectileTriggerEvent;
				SlingshotProjectileHitNotifier.ProjectileTriggerEvent projectileTriggerEvent3 = (SlingshotProjectileHitNotifier.ProjectileTriggerEvent)Delegate.Remove(projectileTriggerEvent2, value);
				projectileTriggerEvent = Interlocked.CompareExchange<SlingshotProjectileHitNotifier.ProjectileTriggerEvent>(ref this.OnProjectileTriggerEnter, projectileTriggerEvent3, projectileTriggerEvent2);
			}
			while (projectileTriggerEvent != projectileTriggerEvent2);
		}
	}

	public event SlingshotProjectileHitNotifier.ProjectileTriggerEvent OnProjectileTriggerExit
	{
		[CompilerGenerated]
		add
		{
			SlingshotProjectileHitNotifier.ProjectileTriggerEvent projectileTriggerEvent = this.OnProjectileTriggerExit;
			SlingshotProjectileHitNotifier.ProjectileTriggerEvent projectileTriggerEvent2;
			do
			{
				projectileTriggerEvent2 = projectileTriggerEvent;
				SlingshotProjectileHitNotifier.ProjectileTriggerEvent projectileTriggerEvent3 = (SlingshotProjectileHitNotifier.ProjectileTriggerEvent)Delegate.Combine(projectileTriggerEvent2, value);
				projectileTriggerEvent = Interlocked.CompareExchange<SlingshotProjectileHitNotifier.ProjectileTriggerEvent>(ref this.OnProjectileTriggerExit, projectileTriggerEvent3, projectileTriggerEvent2);
			}
			while (projectileTriggerEvent != projectileTriggerEvent2);
		}
		[CompilerGenerated]
		remove
		{
			SlingshotProjectileHitNotifier.ProjectileTriggerEvent projectileTriggerEvent = this.OnProjectileTriggerExit;
			SlingshotProjectileHitNotifier.ProjectileTriggerEvent projectileTriggerEvent2;
			do
			{
				projectileTriggerEvent2 = projectileTriggerEvent;
				SlingshotProjectileHitNotifier.ProjectileTriggerEvent projectileTriggerEvent3 = (SlingshotProjectileHitNotifier.ProjectileTriggerEvent)Delegate.Remove(projectileTriggerEvent2, value);
				projectileTriggerEvent = Interlocked.CompareExchange<SlingshotProjectileHitNotifier.ProjectileTriggerEvent>(ref this.OnProjectileTriggerExit, projectileTriggerEvent3, projectileTriggerEvent2);
			}
			while (projectileTriggerEvent != projectileTriggerEvent2);
		}
	}

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

	public void InvokeTriggerEnter(SlingshotProjectile projectile, Collider collider)
	{
		SlingshotProjectileHitNotifier.ProjectileTriggerEvent onProjectileTriggerEnter = this.OnProjectileTriggerEnter;
		if (onProjectileTriggerEnter == null)
		{
			return;
		}
		onProjectileTriggerEnter(projectile, collider);
	}

	public void InvokeTriggerExit(SlingshotProjectile projectile, Collider collider)
	{
		SlingshotProjectileHitNotifier.ProjectileTriggerEvent onProjectileTriggerExit = this.OnProjectileTriggerExit;
		if (onProjectileTriggerExit == null)
		{
			return;
		}
		onProjectileTriggerExit(projectile, collider);
	}

	private new void OnDestroy()
	{
		this.OnProjectileHit = null;
		this.OnProjectileCollisionStay = null;
		this.OnProjectileTriggerExit = null;
	}

	public SlingshotProjectileHitNotifier()
	{
	}

	[CompilerGenerated]
	private SlingshotProjectileHitNotifier.ProjectileHitEvent OnProjectileHit;

	[CompilerGenerated]
	private SlingshotProjectileHitNotifier.ProjectileHitEvent OnProjectileCollisionStay;

	[CompilerGenerated]
	private SlingshotProjectileHitNotifier.ProjectileTriggerEvent OnProjectileTriggerEnter;

	[CompilerGenerated]
	private SlingshotProjectileHitNotifier.ProjectileTriggerEvent OnProjectileTriggerExit;

	public delegate void ProjectileHitEvent(SlingshotProjectile projectile, Collision collision);

	public delegate void ProjectileTriggerEvent(SlingshotProjectile projectile, Collider collider);
}
