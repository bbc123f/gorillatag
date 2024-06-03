using System;
using UnityEngine;

[Serializable]
public class VoiceLoudnessReactorBlendShapeTarget
{
	public VoiceLoudnessReactorBlendShapeTarget()
	{
	}

	public SkinnedMeshRenderer SkinnedMeshRenderer;

	public int BlendShapeIndex;

	public float minValue;

	public float maxValue = 1f;

	public bool UseSmoothedLoudness;
}
