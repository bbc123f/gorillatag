using System;
using UnityEngine;

// Token: 0x020001C9 RID: 457
[Serializable]
public struct FrameStamp
{
	// Token: 0x17000083 RID: 131
	// (get) Token: 0x06000B98 RID: 2968 RVA: 0x00048EA7 File Offset: 0x000470A7
	public int framesElapsed
	{
		get
		{
			return Time.frameCount - this._lastFrame;
		}
	}

	// Token: 0x06000B99 RID: 2969 RVA: 0x00048EB8 File Offset: 0x000470B8
	public static FrameStamp Now()
	{
		return new FrameStamp
		{
			_lastFrame = Time.frameCount
		};
	}

	// Token: 0x06000B9A RID: 2970 RVA: 0x00048EDA File Offset: 0x000470DA
	public override string ToString()
	{
		return string.Format("{0} frames elapsed", this.framesElapsed);
	}

	// Token: 0x06000B9B RID: 2971 RVA: 0x00048EF1 File Offset: 0x000470F1
	public override int GetHashCode()
	{
		return StaticHash.Calculate(this._lastFrame);
	}

	// Token: 0x06000B9C RID: 2972 RVA: 0x00048EFE File Offset: 0x000470FE
	public static implicit operator int(FrameStamp fs)
	{
		return fs.framesElapsed;
	}

	// Token: 0x06000B9D RID: 2973 RVA: 0x00048F08 File Offset: 0x00047108
	public static implicit operator FrameStamp(int framesElapsed)
	{
		return new FrameStamp
		{
			_lastFrame = Time.frameCount - framesElapsed
		};
	}

	// Token: 0x04000F48 RID: 3912
	private int _lastFrame;
}
