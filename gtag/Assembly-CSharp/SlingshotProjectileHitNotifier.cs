using System;
using UnityEngine;

// Token: 0x020000E5 RID: 229
public class SlingshotProjectileHitNotifier : MonoBehaviour
{
	// Token: 0x14000013 RID: 19
	// (add) Token: 0x06000567 RID: 1383 RVA: 0x00022738 File Offset: 0x00020938
	// (remove) Token: 0x06000568 RID: 1384 RVA: 0x00022770 File Offset: 0x00020970
	public event SlingshotProjectileHitNotifier.ProjectileHitEvent OnProjectileHit;

	// Token: 0x14000014 RID: 20
	// (add) Token: 0x06000569 RID: 1385 RVA: 0x000227A8 File Offset: 0x000209A8
	// (remove) Token: 0x0600056A RID: 1386 RVA: 0x000227E0 File Offset: 0x000209E0
	public event SlingshotProjectileHitNotifier.ProjectileHitEvent OnProjectileCollisionStay;

	// Token: 0x0600056B RID: 1387 RVA: 0x00022815 File Offset: 0x00020A15
	public void InvokeHit(SlingshotProjectile projectile, Collision collision)
	{
		SlingshotProjectileHitNotifier.ProjectileHitEvent onProjectileHit = this.OnProjectileHit;
		if (onProjectileHit == null)
		{
			return;
		}
		onProjectileHit(projectile, collision);
	}

	// Token: 0x0600056C RID: 1388 RVA: 0x00022829 File Offset: 0x00020A29
	public void InvokeCollisionStay(SlingshotProjectile projectile, Collision collision)
	{
		SlingshotProjectileHitNotifier.ProjectileHitEvent onProjectileCollisionStay = this.OnProjectileCollisionStay;
		if (onProjectileCollisionStay == null)
		{
			return;
		}
		onProjectileCollisionStay(projectile, collision);
	}

	// Token: 0x0600056D RID: 1389 RVA: 0x0002283D File Offset: 0x00020A3D
	private void OnDestroy()
	{
		this.OnProjectileHit = null;
		this.OnProjectileCollisionStay = null;
	}

	// Token: 0x020003EE RID: 1006
	// (Invoke) Token: 0x06001BD4 RID: 7124
	public delegate void ProjectileHitEvent(SlingshotProjectile projectile, Collision collision);
}
