using System;
using UnityEngine;

public class GorillaSurfaceOverride : MonoBehaviour
{
	[GorillaSoundLookup]
	public int overrideIndex;

	public float extraVelMultiplier = 1f;

	public float extraVelMaxMultiplier = 1f;

	public bool sendOnTapEvent;
}
