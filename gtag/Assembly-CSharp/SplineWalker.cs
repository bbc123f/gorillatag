using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000227 RID: 551
public class SplineWalker : MonoBehaviour, IPunObservable
{
	// Token: 0x06000DA5 RID: 3493 RVA: 0x0004FDB8 File Offset: 0x0004DFB8
	private void Awake()
	{
		this._view = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000DA6 RID: 3494 RVA: 0x0004FDC8 File Offset: 0x0004DFC8
	private void Update()
	{
		if (this.goingForward)
		{
			this.progress += Time.deltaTime / this.duration;
			if (this.progress > 1f)
			{
				if (this.mode == SplineWalkerMode.Once)
				{
					this.progress = 1f;
				}
				else if (this.mode == SplineWalkerMode.Loop)
				{
					this.progress -= 1f;
				}
				else
				{
					this.progress = 2f - this.progress;
					this.goingForward = false;
				}
			}
		}
		else
		{
			this.progress -= Time.deltaTime / this.duration;
			if (this.progress < 0f)
			{
				this.progress = -this.progress;
				this.goingForward = true;
			}
		}
		Vector3 point = this.spline.GetPoint(this.progress);
		base.transform.localPosition = point;
		if (this.lookForward)
		{
			base.transform.LookAt(point + this.spline.GetDirection(this.progress));
		}
	}

	// Token: 0x06000DA7 RID: 3495 RVA: 0x0004FED5 File Offset: 0x0004E0D5
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		stream.Serialize(ref this.progress);
	}

	// Token: 0x040010B9 RID: 4281
	public BezierSpline spline;

	// Token: 0x040010BA RID: 4282
	public float duration;

	// Token: 0x040010BB RID: 4283
	public bool lookForward;

	// Token: 0x040010BC RID: 4284
	public SplineWalkerMode mode;

	// Token: 0x040010BD RID: 4285
	private float progress;

	// Token: 0x040010BE RID: 4286
	private bool goingForward = true;

	// Token: 0x040010BF RID: 4287
	public bool DoNetworkSync = true;

	// Token: 0x040010C0 RID: 4288
	private PhotonView _view;
}
