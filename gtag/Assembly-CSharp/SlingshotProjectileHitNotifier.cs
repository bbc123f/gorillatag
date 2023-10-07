using System;
using UnityEngine;

// Token: 0x020000E5 RID: 229
public class SlingshotProjectileHitNotifier : MonoBehaviour
{
	// Token: 0x14000013 RID: 19
	// (add) Token: 0x06000567 RID: 1383 RVA: 0x00022944 File Offset: 0x00020B44
	// (remove) Token: 0x06000568 RID: 1384 RVA: 0x0002297C File Offset: 0x00020B7C
	public event SlingshotProjectileHitNotifier.ProjectileHitEvent OnProjectileHit;

	// Token: 0x14000014 RID: 20
	// (add) Token: 0x06000569 RID: 1385 RVA: 0x000229B4 File Offset: 0x00020BB4
	// (remove) Token: 0x0600056A RID: 1386 RVA: 0x000229EC File Offset: 0x00020BEC
	public event SlingshotProjectileHitNotifier.ProjectileHitEvent OnProjectileCollisionStay;

	// Token: 0x0600056B RID: 1387 RVA: 0x00022A21 File Offset: 0x00020C21
	public void InvokeHit(SlingshotProjectile projectile, Collision collision)
	{
		SlingshotProjectileHitNotifier.ProjectileHitEvent onProjectileHit = this.OnProjectileHit;
		if (onProjectileHit == null)
		{
			return;
		}
		onProjectileHit(projectile, collision);
	}

	// Token: 0x0600056C RID: 1388 RVA: 0x00022A35 File Offset: 0x00020C35
	public void InvokeCollisionStay(SlingshotProjectile projectile, Collision collision)
	{
		SlingshotProjectileHitNotifier.ProjectileHitEvent onProjectileCollisionStay = this.OnProjectileCollisionStay;
		if (onProjectileCollisionStay == null)
		{
			return;
		}
		onProjectileCollisionStay(projectile, collision);
	}

	// Token: 0x0600056D RID: 1389 RVA: 0x00022A49 File Offset: 0x00020C49
	private void OnDestroy()
	{
		this.OnProjectileHit = null;
		this.OnProjectileCollisionStay = null;
	}

	// Token: 0x020003EC RID: 1004
	// (Invoke) Token: 0x06001BCB RID: 7115
	public delegate void ProjectileHitEvent(SlingshotProjectile projectile, Collision collision);
}
