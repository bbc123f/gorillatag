using System;

public struct TimeSince
{
	public double totalSeconds
	{
		get
		{
			return (DateTime.UtcNow - this._dt).TotalSeconds;
		}
	}

	public float totalSecondsFloat
	{
		get
		{
			return (float)this.totalSeconds;
		}
	}

	public int totalSecondsInt
	{
		get
		{
			return (int)this.totalSeconds;
		}
	}

	public static TimeSince Now()
	{
		return new TimeSince
		{
			_dt = DateTime.UtcNow
		};
	}

	public override string ToString()
	{
		return string.Format("{0:F3} seconds since {{{1:u}}}", this.totalSeconds, this._dt);
	}

	public override int GetHashCode()
	{
		return StaticHash.Calculate(this._dt);
	}

	public static implicit operator double(TimeSince ts)
	{
		return ts.totalSeconds;
	}

	public static implicit operator float(TimeSince ts)
	{
		return ts.totalSecondsFloat;
	}

	public static implicit operator int(TimeSince ts)
	{
		return ts.totalSecondsInt;
	}

	public static implicit operator TimeSince(int seconds)
	{
		return new TimeSince
		{
			_dt = DateTime.UtcNow.AddSeconds((double)(-(double)seconds))
		};
	}

	public static implicit operator TimeSince(float seconds)
	{
		return new TimeSince
		{
			_dt = DateTime.UtcNow.AddSeconds((double)(-(double)seconds))
		};
	}

	public static implicit operator TimeSince(double seconds)
	{
		return new TimeSince
		{
			_dt = DateTime.UtcNow.AddSeconds(-seconds)
		};
	}

	private DateTime _dt;
}
