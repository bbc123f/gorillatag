﻿using System;
using UnityEngine;

// Token: 0x0200007F RID: 127
public class LaserPointer : OVRCursor
{
	// Token: 0x1700001E RID: 30
	// (get) Token: 0x06000289 RID: 649 RVA: 0x00010FBF File Offset: 0x0000F1BF
	// (set) Token: 0x06000288 RID: 648 RVA: 0x00010F8B File Offset: 0x0000F18B
	public LaserPointer.LaserBeamBehavior laserBeamBehavior
	{
		get
		{
			return this._laserBeamBehavior;
		}
		set
		{
			this._laserBeamBehavior = value;
			if (this.laserBeamBehavior == LaserPointer.LaserBeamBehavior.Off || this.laserBeamBehavior == LaserPointer.LaserBeamBehavior.OnWhenHitTarget)
			{
				this.lineRenderer.enabled = false;
				return;
			}
			this.lineRenderer.enabled = true;
		}
	}

	// Token: 0x0600028A RID: 650 RVA: 0x00010FC7 File Offset: 0x0000F1C7
	private void Awake()
	{
		this.lineRenderer = base.GetComponent<LineRenderer>();
	}

	// Token: 0x0600028B RID: 651 RVA: 0x00010FD5 File Offset: 0x0000F1D5
	private void Start()
	{
		if (this.cursorVisual)
		{
			this.cursorVisual.SetActive(false);
		}
		OVRManager.InputFocusAcquired += this.OnInputFocusAcquired;
		OVRManager.InputFocusLost += this.OnInputFocusLost;
	}

	// Token: 0x0600028C RID: 652 RVA: 0x00011012 File Offset: 0x0000F212
	public override void SetCursorStartDest(Vector3 start, Vector3 dest, Vector3 normal)
	{
		this._startPoint = start;
		this._endPoint = dest;
		this._hitTarget = true;
	}

	// Token: 0x0600028D RID: 653 RVA: 0x00011029 File Offset: 0x0000F229
	public override void SetCursorRay(Transform t)
	{
		this._startPoint = t.position;
		this._forward = t.forward;
		this._hitTarget = false;
	}

	// Token: 0x0600028E RID: 654 RVA: 0x0001104C File Offset: 0x0000F24C
	private void LateUpdate()
	{
		this.lineRenderer.SetPosition(0, this._startPoint);
		if (this._hitTarget)
		{
			this.lineRenderer.SetPosition(1, this._endPoint);
			this.UpdateLaserBeam(this._startPoint, this._endPoint);
			if (this.cursorVisual)
			{
				this.cursorVisual.transform.position = this._endPoint;
				this.cursorVisual.SetActive(true);
				return;
			}
		}
		else
		{
			this.UpdateLaserBeam(this._startPoint, this._startPoint + this.maxLength * this._forward);
			this.lineRenderer.SetPosition(1, this._startPoint + this.maxLength * this._forward);
			if (this.cursorVisual)
			{
				this.cursorVisual.SetActive(false);
			}
		}
	}

	// Token: 0x0600028F RID: 655 RVA: 0x00011134 File Offset: 0x0000F334
	private void UpdateLaserBeam(Vector3 start, Vector3 end)
	{
		if (this.laserBeamBehavior == LaserPointer.LaserBeamBehavior.Off)
		{
			return;
		}
		if (this.laserBeamBehavior == LaserPointer.LaserBeamBehavior.On)
		{
			this.lineRenderer.SetPosition(0, start);
			this.lineRenderer.SetPosition(1, end);
			return;
		}
		if (this.laserBeamBehavior == LaserPointer.LaserBeamBehavior.OnWhenHitTarget)
		{
			if (this._hitTarget)
			{
				if (!this.lineRenderer.enabled)
				{
					this.lineRenderer.enabled = true;
					this.lineRenderer.SetPosition(0, start);
					this.lineRenderer.SetPosition(1, end);
					return;
				}
			}
			else if (this.lineRenderer.enabled)
			{
				this.lineRenderer.enabled = false;
			}
		}
	}

	// Token: 0x06000290 RID: 656 RVA: 0x000111CC File Offset: 0x0000F3CC
	private void OnDisable()
	{
		if (this.cursorVisual)
		{
			this.cursorVisual.SetActive(false);
		}
	}

	// Token: 0x06000291 RID: 657 RVA: 0x000111E7 File Offset: 0x0000F3E7
	public void OnInputFocusLost()
	{
		if (base.gameObject && base.gameObject.activeInHierarchy)
		{
			this.m_restoreOnInputAcquired = true;
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x06000292 RID: 658 RVA: 0x00011216 File Offset: 0x0000F416
	public void OnInputFocusAcquired()
	{
		if (this.m_restoreOnInputAcquired && base.gameObject)
		{
			this.m_restoreOnInputAcquired = false;
			base.gameObject.SetActive(true);
		}
	}

	// Token: 0x06000293 RID: 659 RVA: 0x00011240 File Offset: 0x0000F440
	private void OnDestroy()
	{
		OVRManager.InputFocusAcquired -= this.OnInputFocusAcquired;
		OVRManager.InputFocusLost -= this.OnInputFocusLost;
	}

	// Token: 0x0400035F RID: 863
	public GameObject cursorVisual;

	// Token: 0x04000360 RID: 864
	public float maxLength = 10f;

	// Token: 0x04000361 RID: 865
	private LaserPointer.LaserBeamBehavior _laserBeamBehavior;

	// Token: 0x04000362 RID: 866
	private bool m_restoreOnInputAcquired;

	// Token: 0x04000363 RID: 867
	private Vector3 _startPoint;

	// Token: 0x04000364 RID: 868
	private Vector3 _forward;

	// Token: 0x04000365 RID: 869
	private Vector3 _endPoint;

	// Token: 0x04000366 RID: 870
	private bool _hitTarget;

	// Token: 0x04000367 RID: 871
	private LineRenderer lineRenderer;

	// Token: 0x020003AD RID: 941
	public enum LaserBeamBehavior
	{
		// Token: 0x04001B6F RID: 7023
		On,
		// Token: 0x04001B70 RID: 7024
		Off,
		// Token: 0x04001B71 RID: 7025
		OnWhenHitTarget
	}
}
