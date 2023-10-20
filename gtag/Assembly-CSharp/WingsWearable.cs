using System;
using UnityEngine;

// Token: 0x02000035 RID: 53
public class WingsWearable : MonoBehaviour
{
	// Token: 0x06000132 RID: 306 RVA: 0x0000AA9F File Offset: 0x00008C9F
	private void Awake()
	{
		this.xform = this.animator.transform;
	}

	// Token: 0x06000133 RID: 307 RVA: 0x0000AAB2 File Offset: 0x00008CB2
	private void OnEnable()
	{
		this.oldPos = this.xform.localPosition;
	}

	// Token: 0x06000134 RID: 308 RVA: 0x0000AAC8 File Offset: 0x00008CC8
	private void Update()
	{
		Vector3 position = this.xform.position;
		float f = (position - this.oldPos).magnitude / Time.deltaTime;
		float value = this.flapSpeedCurve.Evaluate(Mathf.Abs(f));
		this.animator.SetFloat(this.flapSpeedParamID, value);
		this.oldPos = position;
	}

	// Token: 0x040001A5 RID: 421
	[Tooltip("This animator must have a parameter called 'FlapSpeed'")]
	public Animator animator;

	// Token: 0x040001A6 RID: 422
	[Tooltip("X axis is move speed, Y axis is flap speed")]
	public AnimationCurve flapSpeedCurve;

	// Token: 0x040001A7 RID: 423
	private Transform xform;

	// Token: 0x040001A8 RID: 424
	private Vector3 oldPos;

	// Token: 0x040001A9 RID: 425
	private readonly int flapSpeedParamID = Animator.StringToHash("FlapSpeed");
}
