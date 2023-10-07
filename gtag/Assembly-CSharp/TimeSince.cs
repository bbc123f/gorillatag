using System;

// Token: 0x020001D5 RID: 469
public struct TimeSince
{
	// Token: 0x1700008C RID: 140
	// (get) Token: 0x06000C24 RID: 3108 RVA: 0x0004A1C0 File Offset: 0x000483C0
	public double totalSeconds
	{
		get
		{
			return (DateTime.UtcNow - this._dt).TotalSeconds;
		}
	}

	// Token: 0x1700008D RID: 141
	// (get) Token: 0x06000C25 RID: 3109 RVA: 0x0004A1E5 File Offset: 0x000483E5
	public float totalSecondsFloat
	{
		get
		{
			return (float)this.totalSeconds;
		}
	}

	// Token: 0x1700008E RID: 142
	// (get) Token: 0x06000C26 RID: 3110 RVA: 0x0004A1EE File Offset: 0x000483EE
	public int totalSecondsInt
	{
		get
		{
			return (int)this.totalSeconds;
		}
	}

	// Token: 0x06000C27 RID: 3111 RVA: 0x0004A1F8 File Offset: 0x000483F8
	public static TimeSince Now()
	{
		return new TimeSince
		{
			_dt = DateTime.UtcNow
		};
	}

	// Token: 0x06000C28 RID: 3112 RVA: 0x0004A21A File Offset: 0x0004841A
	public override string ToString()
	{
		return string.Format("{0:F3} seconds since {{{1:u}}}", this.totalSeconds, this._dt);
	}

	// Token: 0x06000C29 RID: 3113 RVA: 0x0004A23C File Offset: 0x0004843C
	public override int GetHashCode()
	{
		return StaticHash.Calculate(this._dt);
	}

	// Token: 0x06000C2A RID: 3114 RVA: 0x0004A249 File Offset: 0x00048449
	public static implicit operator double(TimeSince ts)
	{
		return ts.totalSeconds;
	}

	// Token: 0x06000C2B RID: 3115 RVA: 0x0004A252 File Offset: 0x00048452
	public static implicit operator float(TimeSince ts)
	{
		return ts.totalSecondsFloat;
	}

	// Token: 0x06000C2C RID: 3116 RVA: 0x0004A25B File Offset: 0x0004845B
	public static implicit operator int(TimeSince ts)
	{
		return ts.totalSecondsInt;
	}

	// Token: 0x06000C2D RID: 3117 RVA: 0x0004A264 File Offset: 0x00048464
	public static implicit operator TimeSince(int seconds)
	{
		return new TimeSince
		{
			_dt = DateTime.UtcNow.AddSeconds((double)(-(double)seconds))
		};
	}

	// Token: 0x06000C2E RID: 3118 RVA: 0x0004A294 File Offset: 0x00048494
	public static implicit operator TimeSince(float seconds)
	{
		return new TimeSince
		{
			_dt = DateTime.UtcNow.AddSeconds((double)(-(double)seconds))
		};
	}

	// Token: 0x06000C2F RID: 3119 RVA: 0x0004A2C4 File Offset: 0x000484C4
	public static implicit operator TimeSince(double seconds)
	{
		return new TimeSince
		{
			_dt = DateTime.UtcNow.AddSeconds(-seconds)
		};
	}

	// Token: 0x04000F82 RID: 3970
	private DateTime _dt;
}
