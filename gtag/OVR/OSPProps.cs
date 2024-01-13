using System;
using UnityEngine;

namespace OVR;

[Serializable]
public class OSPProps
{
	[Tooltip("Set to true to play the sound FX spatialized with binaural HRTF, default = false")]
	public bool enableSpatialization;

	[Tooltip("Play the sound FX with reflections, default = false")]
	public bool useFastOverride;

	[Tooltip("Boost the gain on the spatialized sound FX, default = 0.0")]
	[Range(0f, 24f)]
	public float gain;

	[Tooltip("Enable Inverse Square attenuation curve, default = false")]
	public bool enableInvSquare;

	[Tooltip("Change the sound from point source (0.0f) to a spherical volume, default = 0.0")]
	[Range(0f, 1000f)]
	public float volumetric;

	[Tooltip("Set the near and far falloff value for the OSP attenuation curve, default = 1.0")]
	[MinMax(1f, 25f, 0f, 250f)]
	public Vector2 invSquareFalloff = new Vector2(1f, 25f);

	public OSPProps()
	{
		enableSpatialization = false;
		useFastOverride = false;
		gain = 0f;
		enableInvSquare = false;
		volumetric = 0f;
		invSquareFalloff = new Vector2(1f, 25f);
	}
}
