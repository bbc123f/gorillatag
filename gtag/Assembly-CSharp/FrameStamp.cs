using System;
using UnityEngine;

// Token: 0x020001CA RID: 458
[Serializable]
public struct FrameStamp
{
	// Token: 0x17000085 RID: 133
	// (get) Token: 0x06000B9E RID: 2974 RVA: 0x0004910F File Offset: 0x0004730F
	public int framesElapsed
	{
		get
		{
			return Time.frameCount - this._lastFrame;
		}
	}

	// Token: 0x06000B9F RID: 2975 RVA: 0x00049120 File Offset: 0x00047320
	public static FrameStamp Now()
	{
		return new FrameStamp
		{
			_lastFrame = Time.frameCount
		};
	}

	// Token: 0x06000BA0 RID: 2976 RVA: 0x00049142 File Offset: 0x00047342
	public override string ToString()
	{
		return string.Format("{0} frames elapsed", this.framesElapsed);
	}

	// Token: 0x06000BA1 RID: 2977 RVA: 0x00049159 File Offset: 0x00047359
	public override int GetHashCode()
	{
		return StaticHash.Calculate(this._lastFrame);
	}

	// Token: 0x06000BA2 RID: 2978 RVA: 0x00049166 File Offset: 0x00047366
	public static implicit operator int(FrameStamp fs)
	{
		return fs.framesElapsed;
	}

	// Token: 0x06000BA3 RID: 2979 RVA: 0x00049170 File Offset: 0x00047370
	public static implicit operator FrameStamp(int framesElapsed)
	{
		return new FrameStamp
		{
			_lastFrame = Time.frameCount - framesElapsed
		};
	}

	// Token: 0x04000F4C RID: 3916
	private int _lastFrame;
}
