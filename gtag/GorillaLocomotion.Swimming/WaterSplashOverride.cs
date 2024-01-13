using UnityEngine;

namespace GorillaLocomotion.Swimming;

public class WaterSplashOverride : MonoBehaviour
{
	public bool suppressWaterEffects;

	public bool playBigSplash;

	public bool playDrippingEffect = true;

	public bool overrideBoundingRadius;

	public float boundingRadiusOverride = 1f;
}
