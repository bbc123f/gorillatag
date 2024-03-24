using System;
using Photon.Pun;
using UnityEngine;

public class SplineWalker : MonoBehaviour, IPunObservable
{
	private void Awake()
	{
		this._view = base.GetComponent<PhotonView>();
	}

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

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		stream.Serialize(ref this.progress);
	}

	public SplineWalker()
	{
	}

	public BezierSpline spline;

	public float duration;

	public bool lookForward;

	public SplineWalkerMode mode;

	private float progress;

	private bool goingForward = true;

	public bool DoNetworkSync = true;

	private PhotonView _view;
}
