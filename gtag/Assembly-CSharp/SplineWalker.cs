using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000226 RID: 550
public class SplineWalker : MonoBehaviour, IPunObservable
{
	// Token: 0x06000D9F RID: 3487 RVA: 0x0004FB58 File Offset: 0x0004DD58
	private void Awake()
	{
		this._view = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000DA0 RID: 3488 RVA: 0x0004FB68 File Offset: 0x0004DD68
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

	// Token: 0x06000DA1 RID: 3489 RVA: 0x0004FC75 File Offset: 0x0004DE75
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		stream.Serialize(ref this.progress);
	}

	// Token: 0x040010B4 RID: 4276
	public BezierSpline spline;

	// Token: 0x040010B5 RID: 4277
	public float duration;

	// Token: 0x040010B6 RID: 4278
	public bool lookForward;

	// Token: 0x040010B7 RID: 4279
	public SplineWalkerMode mode;

	// Token: 0x040010B8 RID: 4280
	private float progress;

	// Token: 0x040010B9 RID: 4281
	private bool goingForward = true;

	// Token: 0x040010BA RID: 4282
	public bool DoNetworkSync = true;

	// Token: 0x040010BB RID: 4283
	private PhotonView _view;
}
