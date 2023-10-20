using System;
using UnityEngine;

// Token: 0x02000202 RID: 514
public class FPSController : MonoBehaviour
{
	// Token: 0x14000017 RID: 23
	// (add) Token: 0x06000D1E RID: 3358 RVA: 0x0004D5A8 File Offset: 0x0004B7A8
	// (remove) Token: 0x06000D1F RID: 3359 RVA: 0x0004D5E0 File Offset: 0x0004B7E0
	[HideInInspector]
	public event FPSController.OnStateChangeEventHandler OnStartEvent;

	// Token: 0x14000018 RID: 24
	// (add) Token: 0x06000D20 RID: 3360 RVA: 0x0004D618 File Offset: 0x0004B818
	// (remove) Token: 0x06000D21 RID: 3361 RVA: 0x0004D650 File Offset: 0x0004B850
	public event FPSController.OnStateChangeEventHandler OnStopEvent;

	// Token: 0x0400104B RID: 4171
	public float baseMoveSpeed = 4f;

	// Token: 0x0400104C RID: 4172
	public float shiftMoveSpeed = 8f;

	// Token: 0x0400104D RID: 4173
	public float ctrlMoveSpeed = 1f;

	// Token: 0x0400104E RID: 4174
	public float lookHorizontal = 0.4f;

	// Token: 0x0400104F RID: 4175
	public float lookVertical = 0.25f;

	// Token: 0x02000472 RID: 1138
	// (Invoke) Token: 0x06001D47 RID: 7495
	public delegate void OnStateChangeEventHandler();
}
