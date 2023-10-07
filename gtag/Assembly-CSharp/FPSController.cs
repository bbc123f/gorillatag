using System;
using UnityEngine;

// Token: 0x02000201 RID: 513
public class FPSController : MonoBehaviour
{
	// Token: 0x14000017 RID: 23
	// (add) Token: 0x06000D18 RID: 3352 RVA: 0x0004D348 File Offset: 0x0004B548
	// (remove) Token: 0x06000D19 RID: 3353 RVA: 0x0004D380 File Offset: 0x0004B580
	[HideInInspector]
	public event FPSController.OnStateChangeEventHandler OnStartEvent;

	// Token: 0x14000018 RID: 24
	// (add) Token: 0x06000D1A RID: 3354 RVA: 0x0004D3B8 File Offset: 0x0004B5B8
	// (remove) Token: 0x06000D1B RID: 3355 RVA: 0x0004D3F0 File Offset: 0x0004B5F0
	public event FPSController.OnStateChangeEventHandler OnStopEvent;

	// Token: 0x04001046 RID: 4166
	public float baseMoveSpeed = 4f;

	// Token: 0x04001047 RID: 4167
	public float shiftMoveSpeed = 8f;

	// Token: 0x04001048 RID: 4168
	public float ctrlMoveSpeed = 1f;

	// Token: 0x04001049 RID: 4169
	public float lookHorizontal = 0.4f;

	// Token: 0x0400104A RID: 4170
	public float lookVertical = 0.25f;

	// Token: 0x02000470 RID: 1136
	// (Invoke) Token: 0x06001D3E RID: 7486
	public delegate void OnStateChangeEventHandler();
}
