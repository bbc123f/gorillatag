using System;
using UnityEngine;

// Token: 0x0200013F RID: 319
public class GorillaPlaySpace : MonoBehaviour
{
	// Token: 0x17000059 RID: 89
	// (get) Token: 0x06000827 RID: 2087 RVA: 0x00033136 File Offset: 0x00031336
	public static GorillaPlaySpace Instance
	{
		get
		{
			return GorillaPlaySpace._instance;
		}
	}

	// Token: 0x06000828 RID: 2088 RVA: 0x0003313D File Offset: 0x0003133D
	private void Awake()
	{
		if (GorillaPlaySpace._instance != null && GorillaPlaySpace._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		GorillaPlaySpace._instance = this;
	}

	// Token: 0x040009E1 RID: 2529
	private static GorillaPlaySpace _instance;

	// Token: 0x040009E2 RID: 2530
	public Collider headCollider;

	// Token: 0x040009E3 RID: 2531
	public Collider bodyCollider;

	// Token: 0x040009E4 RID: 2532
	public Transform rightHandTransform;

	// Token: 0x040009E5 RID: 2533
	public Transform leftHandTransform;

	// Token: 0x040009E6 RID: 2534
	public Vector3 headColliderOffset;

	// Token: 0x040009E7 RID: 2535
	public Vector3 bodyColliderOffset;

	// Token: 0x040009E8 RID: 2536
	private Vector3 lastLeftHandPosition;

	// Token: 0x040009E9 RID: 2537
	private Vector3 lastRightHandPosition;

	// Token: 0x040009EA RID: 2538
	private Vector3 lastLeftHandPositionForTag;

	// Token: 0x040009EB RID: 2539
	private Vector3 lastRightHandPositionForTag;

	// Token: 0x040009EC RID: 2540
	private Vector3 lastBodyPositionForTag;

	// Token: 0x040009ED RID: 2541
	private Vector3 lastHeadPositionForTag;

	// Token: 0x040009EE RID: 2542
	private Rigidbody playspaceRigidbody;

	// Token: 0x040009EF RID: 2543
	public Transform headsetTransform;

	// Token: 0x040009F0 RID: 2544
	public Vector3 rightHandOffset;

	// Token: 0x040009F1 RID: 2545
	public Vector3 leftHandOffset;

	// Token: 0x040009F2 RID: 2546
	public VRRig vrRig;

	// Token: 0x040009F3 RID: 2547
	public VRRig offlineVRRig;

	// Token: 0x040009F4 RID: 2548
	public float vibrationCooldown = 0.1f;

	// Token: 0x040009F5 RID: 2549
	public float vibrationDuration = 0.05f;

	// Token: 0x040009F6 RID: 2550
	private float leftLastTouchedSurface;

	// Token: 0x040009F7 RID: 2551
	private float rightLastTouchedSurface;

	// Token: 0x040009F8 RID: 2552
	public VRRig myVRRig;

	// Token: 0x040009F9 RID: 2553
	private float bodyHeight;

	// Token: 0x040009FA RID: 2554
	public float tagCooldown;

	// Token: 0x040009FB RID: 2555
	public float taggedTime;

	// Token: 0x040009FC RID: 2556
	public float disconnectTime = 60f;

	// Token: 0x040009FD RID: 2557
	public float maxStepVelocity = 2f;

	// Token: 0x040009FE RID: 2558
	public float hapticWaitSeconds = 0.05f;

	// Token: 0x040009FF RID: 2559
	public float tapHapticDuration = 0.05f;

	// Token: 0x04000A00 RID: 2560
	public float tapHapticStrength = 0.5f;

	// Token: 0x04000A01 RID: 2561
	public float tagHapticDuration = 0.15f;

	// Token: 0x04000A02 RID: 2562
	public float tagHapticStrength = 1f;

	// Token: 0x04000A03 RID: 2563
	public float taggedHapticDuration = 0.35f;

	// Token: 0x04000A04 RID: 2564
	public float taggedHapticStrength = 1f;
}
