using System;

// Token: 0x020001D6 RID: 470
public struct TimeSince
{
	// Token: 0x1700008E RID: 142
	// (get) Token: 0x06000C2A RID: 3114 RVA: 0x0004A428 File Offset: 0x00048628
	public double totalSeconds
	{
		get
		{
			return (DateTime.UtcNow - this._dt).TotalSeconds;
		}
	}

	// Token: 0x1700008F RID: 143
	// (get) Token: 0x06000C2B RID: 3115 RVA: 0x0004A44D File Offset: 0x0004864D
	public float totalSecondsFloat
	{
		get
		{
			return (float)this.totalSeconds;
		}
	}

	// Token: 0x17000090 RID: 144
	// (get) Token: 0x06000C2C RID: 3116 RVA: 0x0004A456 File Offset: 0x00048656
	public int totalSecondsInt
	{
		get
		{
			return (int)this.totalSeconds;
		}
	}

	// Token: 0x06000C2D RID: 3117 RVA: 0x0004A460 File Offset: 0x00048660
	public static TimeSince Now()
	{
		return new TimeSince
		{
			_dt = DateTime.UtcNow
		};
	}

	// Token: 0x06000C2E RID: 3118 RVA: 0x0004A482 File Offset: 0x00048682
	public override string ToString()
	{
		return string.Format("{0:F3} seconds since {{{1:u}}}", this.totalSeconds, this._dt);
	}

	// Token: 0x06000C2F RID: 3119 RVA: 0x0004A4A4 File Offset: 0x000486A4
	public override int GetHashCode()
	{
		return StaticHash.Calculate(this._dt);
	}

	// Token: 0x06000C30 RID: 3120 RVA: 0x0004A4B1 File Offset: 0x000486B1
	public static implicit operator double(TimeSince ts)
	{
		return ts.totalSeconds;
	}

	// Token: 0x06000C31 RID: 3121 RVA: 0x0004A4BA File Offset: 0x000486BA
	public static implicit operator float(TimeSince ts)
	{
		return ts.totalSecondsFloat;
	}

	// Token: 0x06000C32 RID: 3122 RVA: 0x0004A4C3 File Offset: 0x000486C3
	public static implicit operator int(TimeSince ts)
	{
		return ts.totalSecondsInt;
	}

	// Token: 0x06000C33 RID: 3123 RVA: 0x0004A4CC File Offset: 0x000486CC
	public static implicit operator TimeSince(int seconds)
	{
		return new TimeSince
		{
			_dt = DateTime.UtcNow.AddSeconds((double)(-(double)seconds))
		};
	}

	// Token: 0x06000C34 RID: 3124 RVA: 0x0004A4FC File Offset: 0x000486FC
	public static implicit operator TimeSince(float seconds)
	{
		return new TimeSince
		{
			_dt = DateTime.UtcNow.AddSeconds((double)(-(double)seconds))
		};
	}

	// Token: 0x06000C35 RID: 3125 RVA: 0x0004A52C File Offset: 0x0004872C
	public static implicit operator TimeSince(double seconds)
	{
		return new TimeSince
		{
			_dt = DateTime.UtcNow.AddSeconds(-seconds)
		};
	}

	// Token: 0x04000F86 RID: 3974
	private DateTime _dt;
}
