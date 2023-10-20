using System;
using GTMathUtil;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000199 RID: 409
public class MovingPlatform : BasePlatform
{
	// Token: 0x06000A84 RID: 2692 RVA: 0x000413A2 File Offset: 0x0003F5A2
	public float InitTimeOffset()
	{
		return this.startPercentage * this.cycleLength;
	}

	// Token: 0x06000A85 RID: 2693 RVA: 0x000413B1 File Offset: 0x0003F5B1
	private long InitTimeOffsetMs()
	{
		return (long)(this.InitTimeOffset() * 1000f);
	}

	// Token: 0x06000A86 RID: 2694 RVA: 0x000413C0 File Offset: 0x0003F5C0
	private long NetworkTimeMs()
	{
		if (PhotonNetwork.InRoom)
		{
			return (long)((ulong)(PhotonNetwork.ServerTimestamp + int.MinValue) + (ulong)this.InitTimeOffsetMs());
		}
		return (long)(Time.time * 1000f);
	}

	// Token: 0x06000A87 RID: 2695 RVA: 0x000413E9 File Offset: 0x0003F5E9
	private long CycleLengthMs()
	{
		return (long)(this.cycleLength * 1000f);
	}

	// Token: 0x06000A88 RID: 2696 RVA: 0x000413F8 File Offset: 0x0003F5F8
	public double PlatformTime()
	{
		long num = this.NetworkTimeMs();
		long num2 = this.CycleLengthMs();
		return (double)(num - num / num2 * num2) / 1000.0;
	}

	// Token: 0x06000A89 RID: 2697 RVA: 0x00041423 File Offset: 0x0003F623
	public int CycleCount()
	{
		return (int)(this.NetworkTimeMs() / this.CycleLengthMs());
	}

	// Token: 0x06000A8A RID: 2698 RVA: 0x00041434 File Offset: 0x0003F634
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

	// Token: 0x06000A8B RID: 2699 RVA: 0x00041496 File Offset: 0x0003F696
	public bool CycleForward()
	{
		return (this.CycleCount() + (this.startNextCycle ? 1 : 0)) % 2 == 0;
	}

	// Token: 0x06000A8C RID: 2700 RVA: 0x000414B0 File Offset: 0x0003F6B0
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

	// Token: 0x06000A8D RID: 2701 RVA: 0x0004157E File Offset: 0x0003F77E
	private Vector3 UpdatePointToPoint()
	{
		return Vector3.Lerp(this.startPos, this.endPos, this.smoothedPercent);
	}

	// Token: 0x06000A8E RID: 2702 RVA: 0x00041598 File Offset: 0x0003F798
	private Vector3 UpdateArc()
	{
		float angle = Mathf.Lerp(this.rotateStartAmt, this.rotateStartAmt + this.rotateAmt, this.smoothedPercent);
		Quaternion quaternion = this.initLocalRotation;
		Vector3 b = Quaternion.AngleAxis(angle, Vector3.forward) * this.initOffset;
		return this.pivot.transform.position + b;
	}

	// Token: 0x06000A8F RID: 2703 RVA: 0x000415F6 File Offset: 0x0003F7F6
	private Quaternion UpdateRotation()
	{
		return Quaternion.Slerp(this.startRot, this.endRot, this.smoothedPercent);
	}

	// Token: 0x06000A90 RID: 2704 RVA: 0x0004160F File Offset: 0x0003F80F
	private Quaternion UpdateContinuousRotation()
	{
		return Quaternion.AngleAxis(this.smoothedPercent * 360f, Vector3.up) * base.transform.parent.rotation;
	}

	// Token: 0x06000A91 RID: 2705 RVA: 0x0004163C File Offset: 0x0003F83C
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

	// Token: 0x06000A92 RID: 2706 RVA: 0x0004170C File Offset: 0x0003F90C
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

	// Token: 0x06000A93 RID: 2707 RVA: 0x0004181F File Offset: 0x0003FA1F
	public Vector3 ThisFrameMovement()
	{
		return this.deltaPosition;
	}

	// Token: 0x04000D34 RID: 3380
	public MovingPlatform.PlatformType platformType;

	// Token: 0x04000D35 RID: 3381
	public float cycleLength;

	// Token: 0x04000D36 RID: 3382
	public float smoothingHalflife = 0.1f;

	// Token: 0x04000D37 RID: 3383
	public float rotateStartAmt;

	// Token: 0x04000D38 RID: 3384
	public float rotateAmt;

	// Token: 0x04000D39 RID: 3385
	public bool reverseDirOnCycle = true;

	// Token: 0x04000D3A RID: 3386
	public bool reverseDir;

	// Token: 0x04000D3B RID: 3387
	private CriticalSpringDamper springCD = new CriticalSpringDamper();

	// Token: 0x04000D3C RID: 3388
	private Rigidbody rb;

	// Token: 0x04000D3D RID: 3389
	public Transform startXf;

	// Token: 0x04000D3E RID: 3390
	public Transform endXf;

	// Token: 0x04000D3F RID: 3391
	public Vector3 platformInitLocalPos;

	// Token: 0x04000D40 RID: 3392
	private Vector3 startPos;

	// Token: 0x04000D41 RID: 3393
	private Vector3 endPos;

	// Token: 0x04000D42 RID: 3394
	private Quaternion startRot;

	// Token: 0x04000D43 RID: 3395
	private Quaternion endRot;

	// Token: 0x04000D44 RID: 3396
	public float startPercentage;

	// Token: 0x04000D45 RID: 3397
	public float startDelay;

	// Token: 0x04000D46 RID: 3398
	public bool startNextCycle;

	// Token: 0x04000D47 RID: 3399
	public Transform pivot;

	// Token: 0x04000D48 RID: 3400
	private Quaternion initLocalRotation;

	// Token: 0x04000D49 RID: 3401
	private Vector3 initOffset;

	// Token: 0x04000D4A RID: 3402
	private float currT;

	// Token: 0x04000D4B RID: 3403
	private float percent;

	// Token: 0x04000D4C RID: 3404
	private float smoothedPercent = -1f;

	// Token: 0x04000D4D RID: 3405
	private bool currForward;

	// Token: 0x04000D4E RID: 3406
	private float dtSinceServerUpdate;

	// Token: 0x04000D4F RID: 3407
	private double lastServerTime;

	// Token: 0x04000D50 RID: 3408
	public Vector3 currentVelocity;

	// Token: 0x04000D51 RID: 3409
	public Vector3 rotationalAxis;

	// Token: 0x04000D52 RID: 3410
	public float angularVelocity;

	// Token: 0x04000D53 RID: 3411
	public Vector3 rotationPivot;

	// Token: 0x04000D54 RID: 3412
	public Vector3 lastPos;

	// Token: 0x04000D55 RID: 3413
	public Quaternion lastRot;

	// Token: 0x04000D56 RID: 3414
	public Vector3 deltaPosition;

	// Token: 0x04000D57 RID: 3415
	public bool debugMovement;

	// Token: 0x04000D58 RID: 3416
	private double lastNT;

	// Token: 0x04000D59 RID: 3417
	private float lastT;

	// Token: 0x02000444 RID: 1092
	public enum PlatformType
	{
		// Token: 0x04001DAE RID: 7598
		PointToPoint,
		// Token: 0x04001DAF RID: 7599
		Arc,
		// Token: 0x04001DB0 RID: 7600
		Rotation,
		// Token: 0x04001DB1 RID: 7601
		Child,
		// Token: 0x04001DB2 RID: 7602
		ContinuousRotation
	}
}
