using System;
using GTMathUtil;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000198 RID: 408
public class MovingPlatform : BasePlatform
{
	// Token: 0x06000A7F RID: 2687 RVA: 0x0004126A File Offset: 0x0003F46A
	public float InitTimeOffset()
	{
		return this.startPercentage * this.cycleLength;
	}

	// Token: 0x06000A80 RID: 2688 RVA: 0x00041279 File Offset: 0x0003F479
	private long InitTimeOffsetMs()
	{
		return (long)(this.InitTimeOffset() * 1000f);
	}

	// Token: 0x06000A81 RID: 2689 RVA: 0x00041288 File Offset: 0x0003F488
	private long NetworkTimeMs()
	{
		if (PhotonNetwork.InRoom)
		{
			return (long)((ulong)(PhotonNetwork.ServerTimestamp + int.MinValue) + (ulong)this.InitTimeOffsetMs());
		}
		return (long)(Time.time * 1000f);
	}

	// Token: 0x06000A82 RID: 2690 RVA: 0x000412B1 File Offset: 0x0003F4B1
	private long CycleLengthMs()
	{
		return (long)(this.cycleLength * 1000f);
	}

	// Token: 0x06000A83 RID: 2691 RVA: 0x000412C0 File Offset: 0x0003F4C0
	public double PlatformTime()
	{
		long num = this.NetworkTimeMs();
		long num2 = this.CycleLengthMs();
		return (double)(num - num / num2 * num2) / 1000.0;
	}

	// Token: 0x06000A84 RID: 2692 RVA: 0x000412EB File Offset: 0x0003F4EB
	public int CycleCount()
	{
		return (int)(this.NetworkTimeMs() / this.CycleLengthMs());
	}

	// Token: 0x06000A85 RID: 2693 RVA: 0x000412FC File Offset: 0x0003F4FC
	public float CycleCompletionPercent()
	{
		float num = (float)(this.PlatformTime() / (double)this.cycleLength);
		num = Mathf.Clamp(num, 0f, 1f);
		if (this.startDelay > 0f)
		{
			float num2 = this.startDelay / this.cycleLength;
			if (num <= num2)
			{
				num = 0f;
			}
			else
			{
				num = (num - num2) / (1f - num2);
			}
		}
		return num;
	}

	// Token: 0x06000A86 RID: 2694 RVA: 0x0004135E File Offset: 0x0003F55E
	public bool CycleForward()
	{
		return (this.CycleCount() + (this.startNextCycle ? 1 : 0)) % 2 == 0;
	}

	// Token: 0x06000A87 RID: 2695 RVA: 0x00041378 File Offset: 0x0003F578
	private void Awake()
	{
		if (this.platformType == MovingPlatform.PlatformType.Child)
		{
			return;
		}
		this.rb = base.GetComponent<Rigidbody>();
		this.initLocalRotation = base.transform.localRotation;
		if (this.pivot != null)
		{
			this.initOffset = this.pivot.transform.position - this.startXf.transform.position;
		}
		this.startPos = this.startXf.position;
		this.endPos = this.endXf.position;
		this.startRot = this.startXf.rotation;
		this.endRot = this.endXf.rotation;
		this.platformInitLocalPos = base.transform.localPosition;
		this.currT = this.startPercentage;
	}

	// Token: 0x06000A88 RID: 2696 RVA: 0x00041446 File Offset: 0x0003F646
	private Vector3 UpdatePointToPoint()
	{
		return Vector3.Lerp(this.startPos, this.endPos, this.smoothedPercent);
	}

	// Token: 0x06000A89 RID: 2697 RVA: 0x00041460 File Offset: 0x0003F660
	private Vector3 UpdateArc()
	{
		float angle = Mathf.Lerp(this.rotateStartAmt, this.rotateStartAmt + this.rotateAmt, this.smoothedPercent);
		Quaternion quaternion = this.initLocalRotation;
		Vector3 b = Quaternion.AngleAxis(angle, Vector3.forward) * this.initOffset;
		return this.pivot.transform.position + b;
	}

	// Token: 0x06000A8A RID: 2698 RVA: 0x000414BE File Offset: 0x0003F6BE
	private Quaternion UpdateRotation()
	{
		return Quaternion.Slerp(this.startRot, this.endRot, this.smoothedPercent);
	}

	// Token: 0x06000A8B RID: 2699 RVA: 0x000414D7 File Offset: 0x0003F6D7
	private Quaternion UpdateContinuousRotation()
	{
		return Quaternion.AngleAxis(this.smoothedPercent * 360f, Vector3.up) * base.transform.parent.rotation;
	}

	// Token: 0x06000A8C RID: 2700 RVA: 0x00041504 File Offset: 0x0003F704
	private void SetupContext()
	{
		double time = PhotonNetwork.Time;
		if (this.lastServerTime == time)
		{
			this.dtSinceServerUpdate += Time.fixedDeltaTime;
		}
		else
		{
			this.dtSinceServerUpdate = 0f;
			this.lastServerTime = time;
		}
		float num = this.currT;
		this.currT = this.CycleCompletionPercent();
		this.currForward = this.CycleForward();
		this.percent = this.currT;
		if (this.reverseDirOnCycle)
		{
			this.percent = (this.currForward ? this.currT : (1f - this.currT));
		}
		if (this.reverseDir)
		{
			this.percent = 1f - this.percent;
		}
		this.smoothedPercent = this.percent;
		this.lastNT = time;
		this.lastT = Time.time;
	}

	// Token: 0x06000A8D RID: 2701 RVA: 0x000415D4 File Offset: 0x0003F7D4
	private void LateUpdate()
	{
		if (this.platformType == MovingPlatform.PlatformType.Child)
		{
			return;
		}
		this.SetupContext();
		Vector3 vector = base.transform.position;
		Quaternion quaternion = base.transform.rotation;
		switch (this.platformType)
		{
		case MovingPlatform.PlatformType.PointToPoint:
			vector = this.UpdatePointToPoint();
			break;
		case MovingPlatform.PlatformType.Arc:
			vector = this.UpdateArc();
			break;
		case MovingPlatform.PlatformType.Rotation:
			quaternion = this.UpdateRotation();
			break;
		case MovingPlatform.PlatformType.ContinuousRotation:
			quaternion = this.UpdateContinuousRotation();
			break;
		}
		if (!this.debugMovement)
		{
			this.lastPos = this.rb.position;
			this.lastRot = this.rb.rotation;
			if (this.platformType != MovingPlatform.PlatformType.Rotation)
			{
				this.rb.MovePosition(vector);
			}
			this.rb.MoveRotation(quaternion);
		}
		else
		{
			this.lastPos = base.transform.position;
			this.lastRot = base.transform.rotation;
			base.transform.position = vector;
			base.transform.rotation = quaternion;
		}
		this.deltaPosition = vector - this.lastPos;
	}

	// Token: 0x06000A8E RID: 2702 RVA: 0x000416E7 File Offset: 0x0003F8E7
	public Vector3 ThisFrameMovement()
	{
		return this.deltaPosition;
	}

	// Token: 0x04000D30 RID: 3376
	public MovingPlatform.PlatformType platformType;

	// Token: 0x04000D31 RID: 3377
	public float cycleLength;

	// Token: 0x04000D32 RID: 3378
	public float smoothingHalflife = 0.1f;

	// Token: 0x04000D33 RID: 3379
	public float rotateStartAmt;

	// Token: 0x04000D34 RID: 3380
	public float rotateAmt;

	// Token: 0x04000D35 RID: 3381
	public bool reverseDirOnCycle = true;

	// Token: 0x04000D36 RID: 3382
	public bool reverseDir;

	// Token: 0x04000D37 RID: 3383
	private CriticalSpringDamper springCD = new CriticalSpringDamper();

	// Token: 0x04000D38 RID: 3384
	private Rigidbody rb;

	// Token: 0x04000D39 RID: 3385
	public Transform startXf;

	// Token: 0x04000D3A RID: 3386
	public Transform endXf;

	// Token: 0x04000D3B RID: 3387
	public Vector3 platformInitLocalPos;

	// Token: 0x04000D3C RID: 3388
	private Vector3 startPos;

	// Token: 0x04000D3D RID: 3389
	private Vector3 endPos;

	// Token: 0x04000D3E RID: 3390
	private Quaternion startRot;

	// Token: 0x04000D3F RID: 3391
	private Quaternion endRot;

	// Token: 0x04000D40 RID: 3392
	public float startPercentage;

	// Token: 0x04000D41 RID: 3393
	public float startDelay;

	// Token: 0x04000D42 RID: 3394
	public bool startNextCycle;

	// Token: 0x04000D43 RID: 3395
	public Transform pivot;

	// Token: 0x04000D44 RID: 3396
	private Quaternion initLocalRotation;

	// Token: 0x04000D45 RID: 3397
	private Vector3 initOffset;

	// Token: 0x04000D46 RID: 3398
	private float currT;

	// Token: 0x04000D47 RID: 3399
	private float percent;

	// Token: 0x04000D48 RID: 3400
	private float smoothedPercent = -1f;

	// Token: 0x04000D49 RID: 3401
	private bool currForward;

	// Token: 0x04000D4A RID: 3402
	private float dtSinceServerUpdate;

	// Token: 0x04000D4B RID: 3403
	private double lastServerTime;

	// Token: 0x04000D4C RID: 3404
	public Vector3 currentVelocity;

	// Token: 0x04000D4D RID: 3405
	public Vector3 rotationalAxis;

	// Token: 0x04000D4E RID: 3406
	public float angularVelocity;

	// Token: 0x04000D4F RID: 3407
	public Vector3 rotationPivot;

	// Token: 0x04000D50 RID: 3408
	public Vector3 lastPos;

	// Token: 0x04000D51 RID: 3409
	public Quaternion lastRot;

	// Token: 0x04000D52 RID: 3410
	public Vector3 deltaPosition;

	// Token: 0x04000D53 RID: 3411
	public bool debugMovement;

	// Token: 0x04000D54 RID: 3412
	private double lastNT;

	// Token: 0x04000D55 RID: 3413
	private float lastT;

	// Token: 0x02000442 RID: 1090
	public enum PlatformType
	{
		// Token: 0x04001DA1 RID: 7585
		PointToPoint,
		// Token: 0x04001DA2 RID: 7586
		Arc,
		// Token: 0x04001DA3 RID: 7587
		Rotation,
		// Token: 0x04001DA4 RID: 7588
		Child,
		// Token: 0x04001DA5 RID: 7589
		ContinuousRotation
	}
}
