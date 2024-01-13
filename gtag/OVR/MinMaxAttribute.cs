using UnityEngine;

namespace OVR;

public class MinMaxAttribute : PropertyAttribute
{
	public float minDefaultVal = 1f;

	public float maxDefaultVal = 1f;

	public float min;

	public float max = 1f;

	public MinMaxAttribute(float minDefaultVal, float maxDefaultVal, float min, float max)
	{
		this.minDefaultVal = minDefaultVal;
		this.maxDefaultVal = maxDefaultVal;
		this.min = min;
		this.max = max;
	}
}
