using System;

namespace GT;

public struct TimeSince
{
	private DateTime _dt;

	public double totalSeconds => (DateTime.UtcNow - _dt).TotalSeconds;

	public float totalSecondsFloat => (float)totalSeconds;

	public static TimeSince Now()
	{
		TimeSince result = default(TimeSince);
		result._dt = DateTime.UtcNow;
		return result;
	}

	public static implicit operator float(TimeSince ts)
	{
		return ts.totalSecondsFloat;
	}

	public static implicit operator double(TimeSince ts)
	{
		return ts.totalSeconds;
	}

	public static implicit operator TimeSince(float seconds)
	{
		TimeSince result = default(TimeSince);
		result._dt = DateTime.UtcNow.AddSeconds(0f - seconds);
		return result;
	}

	public static implicit operator TimeSince(double seconds)
	{
		TimeSince result = default(TimeSince);
		result._dt = DateTime.UtcNow.AddSeconds(0.0 - seconds);
		return result;
	}
}
